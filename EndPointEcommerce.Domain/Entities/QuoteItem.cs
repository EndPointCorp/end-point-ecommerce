using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity that holds the item information for a given quote.
/// </summary>
public class QuoteItem : BaseEntity, IQuoteItem
{
    public Quote Quote { get; set; } = default!;
    public int QuoteId { get; set; }

    public Product Product { get; set; } = default!;
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Display(Name = "Unit price")]
    public decimal UnitPrice => Product.GetActualPrice();
    [Display(Name = "Total price")]
    public decimal TotalPrice => UnitPrice * Quantity;
    public decimal Discount => Quote.GetDiscountForItem(this);
    public decimal Total => TotalPrice - Discount;

    public static QuoteItem Build(Quote quote, Product product, int quantity)
    {
        return new()
        {
            QuoteId = quote.Id,
            Quote = quote,

            ProductId = product.Id,
            Product = product,

            Quantity = quantity,
        };
    }

    public QuoteItem Clone()
    {
        return new()
        {
            ProductId = ProductId,
            Product = Product,
            Quantity = Quantity
        };
    }
}
