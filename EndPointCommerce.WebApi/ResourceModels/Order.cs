namespace EndPointCommerce.WebApi.ResourceModels;

public class Order
{
    public int Id { get; set; }
    public DateTime? DateCreated { get; set; }

    public ICollection<QuoteItem> Items { get; set; } = [];

    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public QuoteAddress ShippingAddress { get; set; } = default!;
    public QuoteAddress BillingAddress { get; set; } = default!;

    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;

    public string? TrackingNumber { get; set; }

    public static Order FromEntity(Domain.Entities.Order entity, string? imagesUrlPath = null)
    {
        var order = new Order()
        {
            Id = entity.Id,
            DateCreated = entity.DateCreated,
            TrackingNumber = entity.TrackingNumber,

            Subtotal = entity.Subtotal,
            Discount = entity.Discount,
            Tax = entity.Tax,
            Total = entity.Total
        };

        if (entity.Items != null)
            order.Items = QuoteItem.FromListOfEntities([.. entity.Items], imagesUrlPath);

        if (entity.ShippingAddress != null)
            order.ShippingAddress = QuoteAddress.FromEntity(entity.ShippingAddress);

        if (entity.BillingAddress != null)
            order.BillingAddress = QuoteAddress.FromEntity(entity.BillingAddress);

        if (entity.Status != null) order.Status = entity.Status.Name;
        if (entity.PaymentMethod != null) order.PaymentMethod = entity.PaymentMethod.Name;

        return order;
    }

    public static List<Order> FromListOfEntities(ICollection<Domain.Entities.Order> entities, string? imagesUrlPath = null) =>
        entities.Select(e => FromEntity(e, imagesUrlPath)).ToList();
}
