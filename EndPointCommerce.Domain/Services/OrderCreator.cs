using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using static EndPointCommerce.Domain.Services.IOrderCreator;

namespace EndPointCommerce.Domain.Services;

public interface IOrderCreator
{
    public class InputPayload
    {
        public required int QuoteId { get; set; }
        public string? PaymentMethodNonceValue { get; set; }
        public string? PaymentMethodNonceDescriptor { get; set; }
    }

    Task<Order> Run(InputPayload payload);
}

public class OrderCreator : IOrderCreator
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IOrderConfirmationMailer _orderConfirmationMailer;
    private readonly IPaymentGateway _paymentGateway;

    public OrderCreator(
        IQuoteRepository quoteRepository,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IAddressRepository addressRepository,
        IOrderStatusRepository orderStatusRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IOrderConfirmationMailer orderConfirmationMailer,
        IPaymentGateway paymentGateway
    ) {
        _quoteRepository = quoteRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _addressRepository = addressRepository;
        _orderStatusRepository = orderStatusRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _orderConfirmationMailer = orderConfirmationMailer;
        _paymentGateway = paymentGateway;
    }

    public async Task<Order> Run(InputPayload payload)
    {
        var quote = await FindQuoteOrThrow(payload);
        ThrowIfNotValid(quote);

        if (!quote.IsFree()) ThrowIfPaymentNotValid(payload);

        var customer = await FindOrCreateCustomer(quote);
        var pending = await _orderStatusRepository.GetPending();

        var paymentMethod = quote.IsFree() ?
            await _paymentMethodRepository.GetFreeOrder() :
            await _paymentMethodRepository.GetCreditCard();

        var order = Order.Build(
            customer, quote, pending, paymentMethod,
            payload.PaymentMethodNonceValue!, payload.PaymentMethodNonceDescriptor!
        );

        ProcessPayment(order);

        await _orderRepository.AddAsync(order);

        quote.Close();
        await _quoteRepository.UpdateAsync(quote);

        await _orderConfirmationMailer.SendAsync(order);

        return order;
    }

    private void ProcessPayment(Order order)
    {
        var result = _paymentGateway.CreatePaymentTransaction(order);
        if (!result.IsSuccess) throw new DomainValidationException("Error processing payment");

        order.PaymentTransactionId = result.TransactionId;
    }

    private async Task<Quote> FindQuoteOrThrow(InputPayload payload) =>
        await _quoteRepository.FindOpenByIdAsync(payload.QuoteId) ??
            throw new EntityNotFoundException();

    private static void ThrowIfNotValid(Quote quote)
    {
        var (isValid, results) = quote.Validate();

        if (!isValid)
            throw new DomainValidationException("The quote is not valid", results);
    }

    private void ThrowIfPaymentNotValid(InputPayload payload)
    {
        if (string.IsNullOrEmpty(payload.PaymentMethodNonceValue))
            throw new DomainValidationException("The PaymentMethodNonceValue is required for non-free orders");

        if (string.IsNullOrEmpty(payload.PaymentMethodNonceDescriptor))
            throw new DomainValidationException("The PaymentMethodNonceDescriptor is required for non-free orders");
    }

    private async Task<Customer> FindOrCreateCustomer(Quote quote)
    {
        Customer? customer;

        if (quote.IsFromCustomer)
        {
            customer = quote.Customer!;
        }
        else
        {
            customer = await _customerRepository.FindByEmailAsync(quote.Email!);

            if (customer == null)
            {
                customer = new Customer
                {
                    Email = quote.Email!,
                    Name = quote.Email!
                };

                await _customerRepository.AddAsync(customer);
            }

            quote.CustomerId = customer.Id;
            await _quoteRepository.UpdateAsync(quote);
        }

        await AssignAddresses(quote, customer);

        return customer;
    }

    private async Task AssignAddresses(Quote quote, Customer customer)
    {
        var customerBillingAddress = quote.BillingAddress;
        customerBillingAddress!.CustomerId = customer.Id;
        await _addressRepository.UpdateAsync(customerBillingAddress);

        var customerShippingAddress = quote.ShippingAddress;
        customerShippingAddress!.CustomerId = customer.Id;
        await _addressRepository.UpdateAsync(customerShippingAddress);
    }
}
