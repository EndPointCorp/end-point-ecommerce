namespace EndPointEcommerce.WebStore.Api;

public class QuoteItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public Product Product { get; set; } = default!;
}
