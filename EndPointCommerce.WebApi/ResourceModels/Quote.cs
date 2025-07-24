namespace EndPointCommerce.WebApi.ResourceModels;

public class Quote
{
    public int Id { get; set; }
    public Coupon? Coupon { get; set; }
    public int? CouponId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public string? Email { get; set; }
    public QuoteAddress? ShippingAddress { get; set; }
    public QuoteAddress? BillingAddress { get; set; }

    public ICollection<QuoteItem> Items { get; set; } = [];

    public static Quote FromEntity(Domain.Entities.Quote entity, string? imagesUrl = null)
    {
        return new() {
            Id = entity.Id,

            Subtotal = entity.Subtotal,
            Discount = entity.Discount,
            Tax = entity.Tax,
            Total = entity.Total,

            CouponId = entity.CouponId,
            Coupon = entity.Coupon is null ? null : Coupon.FromEntity(entity.Coupon),

            Email = entity.Email,
            ShippingAddress = entity.ShippingAddress is null ? null : QuoteAddress.FromEntity(entity.ShippingAddress),
            BillingAddress = entity.BillingAddress is null ? null : QuoteAddress.FromEntity(entity.BillingAddress),

            Items = QuoteItem.FromListOfEntities([.. entity.Items], imagesUrl)
        };
    }
}
