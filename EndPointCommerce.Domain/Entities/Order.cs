using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.Domain.Entities;

/// <summary>
/// Entity that holds the order information for a purchase.
/// </summary>
public class Order : BaseAuditEntity
{
    public Customer Customer { get; set; } = default!;
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }
    public Coupon? Coupon { get; set; }
    [Display(Name = "Coupon")]
    public int? CouponId { get; set; }

    public Quote? Quote { get; set; }
    [Display(Name = "Quote")]
    public int? QuoteId { get; set; }

    public IList<OrderItem> Items { get; set; } = [];

    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public Address ShippingAddress { get; set; } = default!;
    [Required]
    [Display(Name = "Shipping Address")]
    public int ShippingAddressId { get; set; }

    public Address BillingAddress { get; set; } = default!;
    [Required]
    [Display(Name = "Billing Address")]
    public int BillingAddressId { get; set; }

    public OrderStatus Status { get; set; } = default!;
    [Display(Name = "Status")]
    public int StatusId { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = default!;
    [Required]
    [Display(Name = "Payment Method")]
    public int PaymentMethodId { get; set; }

    public string? PaymentMethodNonceValue { get; set; }
    public string? PaymentMethodNonceDescriptor { get; set; }
    public string? PaymentTransactionId { get; set; }

    [Display(Name = "Tracking Number")]
    public string? TrackingNumber { get; set; }

    public static Order Build(
        Customer customer,
        Quote quote,
        OrderStatus status,
        PaymentMethod paymentMethod,
        string? PaymentMethodNonceValue,
        string? PaymentMethodNonceDescriptor
    ) =>
        new()
        {
            CustomerId = customer.Id,
            QuoteId = quote.Id,

            CouponId = quote.CouponId,
            Coupon = quote.Coupon,

            Items = quote.Items
                .Select(OrderItem.Build)
                .ToList(),

            Subtotal = quote.Subtotal,
            Discount = quote.Discount,
            Tax = quote.Tax,
            Total = quote.Total,

            ShippingAddress = quote.ShippingAddress!.Clone(),
            BillingAddress = quote.BillingAddress!.Clone(),

            Status = status,
            PaymentMethod = paymentMethod,
            PaymentMethodNonceValue = PaymentMethodNonceValue,
            PaymentMethodNonceDescriptor = PaymentMethodNonceDescriptor
        };
}