// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Tests.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Build_ShouldCreateANewOrderWithCorrectProperties()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "test_name",
            Email = "test@email.com"
        };

        var quote = new Quote
        {
            Id = 10,

            CouponId = 100,
            Coupon = new Coupon
            {
                Code = "test_code",
                Discount = 10,
                IsDiscountFixed = true
            },

            Tax = 5,

            ShippingAddress = new Address
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "123 Main St",
                City = "test_city",
                ZipCode = "12345",
                CountryId = 1,
                Country = new Country
                {
                    Id = 1,
                    Name = "test_country",
                    Code = "TC"
                },
                StateId = 1,
                State = new State
                {
                    Id = 1,
                    Name = "test_state",
                    Abbreviation = "TS"
                }
            },
            
            BillingAddress = new Address
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "456 Main St",
                City = "test_city",
                ZipCode = "54321",
                CountryId = 1,
                Country = new Country
                {
                    Id = 1,
                    Name = "test_country",
                    Code = "TC"
                },
                StateId = 1,
                State = new State
                {
                    Id = 1,
                    Name = "test_state",
                    Abbreviation = "TS"
                }
            }
        };

        quote.Items = [
            new QuoteItem
            {
                Id = 1,
                Quote = quote,
                Quantity = 2,
                ProductId = 1,
                Product = new Product
                {
                    Name = "test_name",
                    Sku = "test_sku",
                    BasePrice = 100M,
                }
            }
        ];

        var status = new OrderStatus { Id = 1, Name = OrderStatus.PENDING };
        var paymentMethod = new PaymentMethod { Id = 1, Name = PaymentMethod.CREDIT_CARD };
        var paymentMethodNonceValue = "test_payment_nonce_value";
        var paymentMethodNonceDescriptor = "test_payment_nonce_descriptor";

        // Act
        var order = Order.Build(
            customer, quote, status, paymentMethod,
            paymentMethodNonceValue, paymentMethodNonceDescriptor
        );

        // Assert
        Assert.Equal(customer.Id, order.CustomerId);
        Assert.Equal(quote.Id, order.QuoteId);
        Assert.Equal(quote.CouponId, order.CouponId);
        Assert.Equal(quote.Coupon, order.Coupon);
        Assert.Equal(quote.Items.Count, order.Items.Count);
        Assert.Equal(quote.Subtotal, order.Subtotal);
        Assert.Equal(quote.Discount, order.Discount);
        Assert.Equal(quote.Tax, order.Tax);
        Assert.Equal(quote.Total, order.Total);
        Assert.Equal(quote.ShippingAddress.Street, order.ShippingAddress.Street);
        Assert.Equal(quote.BillingAddress.Street, order.BillingAddress.Street);
        Assert.Equal(status, order.Status);
        Assert.Equal(paymentMethod, order.PaymentMethod);
        Assert.Equal(paymentMethodNonceValue, order.PaymentMethodNonceValue);
        Assert.Equal(paymentMethodNonceDescriptor, order.PaymentMethodNonceDescriptor);
    }
}
