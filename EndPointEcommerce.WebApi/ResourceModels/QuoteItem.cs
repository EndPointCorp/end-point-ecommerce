namespace EndPointEcommerce.WebApi.ResourceModels;

public class QuoteItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public QuoteItemProduct Product { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }

    public static QuoteItem FromEntity(Domain.Interfaces.IQuoteItem entity, string? imagesUrl = null)
    {
        return new()
        {
            Id = entity.Id,

            ProductId = entity.ProductId,
            Product = QuoteItemProduct.FromEntity(entity.Product, imagesUrl),

            Quantity = entity.Quantity,

            UnitPrice = entity.UnitPrice,
            TotalPrice = entity.TotalPrice,
            Discount = entity.Discount,
            Total = entity.Total
        };
    }

    public static List<QuoteItem> FromListOfEntities(
        IList<Domain.Interfaces.IQuoteItem> entities,
        string? imagesUrl = null
    ) =>
        entities.Select(e => FromEntity(e, imagesUrl)).ToList();
}
