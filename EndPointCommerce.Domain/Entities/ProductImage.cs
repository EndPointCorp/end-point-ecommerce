namespace EndPointCommerce.Domain.Entities;

public class ProductImage : Image
{
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
}