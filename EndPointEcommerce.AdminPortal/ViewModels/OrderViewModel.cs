using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for orders.
/// </summary>
public class OrderViewModel : Order
{
    public IEnumerable<SelectListItem>? Customers { get; set; }
    public IEnumerable<SelectListItem>? Statuses { get; set; }
    public IEnumerable<SelectListItem>? PaymentMethods { get; set; }

    public Order ToModel()
    {
        return new Order() {
            Id = Id,
            QuoteId = QuoteId,
            CouponId = CouponId,
            StatusId = StatusId,
            CustomerId = CustomerId,
            ShippingAddressId = ShippingAddressId,
            BillingAddressId = BillingAddressId,
            PaymentMethodId = PaymentMethodId,
            PaymentTransactionId = PaymentTransactionId,
            TrackingNumber = TrackingNumber,
            Subtotal = Subtotal,
            Discount = Discount,
            Tax = Tax,
            Total = Total,
            Items = Items,
        };
    }

    public async Task FillStatuses(IOrderStatusRepository orderStatusRepository)
    {
        var statuses = await orderStatusRepository.FetchAllAsync();
        Statuses =
            statuses.
            Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
    }

    public async Task FillCustomers(ICustomerRepository customerRepository)
    {
        var customers = await customerRepository.FetchAllAsync();
        Customers =
            customers.
            Select(x => new SelectListItem()
            {
                Text = x.FullName,
                Value = x.Id.ToString()
            }).ToList();
    }

    public async Task FillPaymentMethods(IPaymentMethodRepository paymentMethodRepository)
    {
        var paymentMethods = await paymentMethodRepository.FetchAllAsync();
        PaymentMethods =
            paymentMethods.
            Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
    }

    public static async Task<OrderViewModel> CreateDefault(int shippingAddressId, int billingAddressId, int customerId, int paymentMethodId, int statusId,
        IOrderStatusRepository orderStatusRepository, ICustomerRepository customerRepository, IPaymentMethodRepository paymentMethodRepository)
    {
        var orderViewModel = new OrderViewModel() {
            ShippingAddressId = shippingAddressId,
            BillingAddressId = billingAddressId,
            CustomerId = customerId,
            PaymentMethodId = paymentMethodId,
            StatusId = statusId,
        };
        await orderViewModel.FillStatuses(orderStatusRepository);
        await orderViewModel.FillCustomers(customerRepository);
        await orderViewModel.FillPaymentMethods(paymentMethodRepository);
        return orderViewModel;
    }

    public static async Task<OrderViewModel> FromModel(Order model, IOrderStatusRepository orderStatusRepository, ICustomerRepository customerRepository,
        IPaymentMethodRepository paymentMethodRepository)
    {
        var orderViewModel = await CreateDefault(
            model.ShippingAddressId, model.BillingAddressId,
            model.CustomerId, model.PaymentMethodId, model.StatusId,
            orderStatusRepository, customerRepository, paymentMethodRepository
        );

        orderViewModel.Id = model.Id;
        orderViewModel.QuoteId = model.QuoteId;
        orderViewModel.CouponId = model.CouponId;
        orderViewModel.StatusId = model.StatusId;
        orderViewModel.CustomerId = model.CustomerId;
        orderViewModel.ShippingAddressId = model.ShippingAddressId;
        orderViewModel.BillingAddressId = model.BillingAddressId;
        orderViewModel.PaymentMethodId = model.PaymentMethodId;
        orderViewModel.PaymentTransactionId = model.PaymentTransactionId;
        orderViewModel.TrackingNumber = model.TrackingNumber;
        orderViewModel.Subtotal = model.Subtotal;
        orderViewModel.Discount = model.Discount;
        orderViewModel.Tax = model.Tax;
        orderViewModel.Total = model.Total;
        orderViewModel.Items = model.Items.ToList();
        return orderViewModel;
    }
}