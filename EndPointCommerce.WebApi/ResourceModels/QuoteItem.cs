namespace EndPointCommerce.WebApi.ResourceModels;

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

    public static QuoteItem FromEntity(Domain.Interfaces.IQuoteItem entity, string? imagesUrlPath = null)
    {
        return new()
        {
            Id = entity.Id,

            ProductId = entity.ProductId,
            Product = QuoteItemProduct.FromEntity(entity.Product, imagesUrlPath),

            Quantity = entity.Quantity,

            UnitPrice = entity.UnitPrice,
            TotalPrice = entity.TotalPrice,
            Discount = entity.Discount,
            Total = entity.Total
        };
    }

    public static List<QuoteItem> FromListOfEntities(
        IList<Domain.Interfaces.IQuoteItem> entities,
        string? imagesUrlPath = null
    ) =>
        entities.Select(e => FromEntity(e, imagesUrlPath)).ToList();
}
