using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Validation;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity that holds the quote/cart information before a purchase is completed.
/// </summary>
public class Quote: BaseAuditEntity
{
    public Customer? Customer { get; set; }
    public int? CustomerId { get; set; }
    public Coupon? Coupon { get; set; }
    public int? CouponId { get; set; }

    [Display(Name = "Is open?")]
    public bool IsOpen { get; set; } = true;

    public string? Email { get; set; }

    public Address? ShippingAddress { get; set; }
    public int? ShippingAddressId { get; set; }

    public Address? BillingAddress { get; set; }
    public int? BillingAddressId { get; set; }

    public IList<QuoteItem> Items { get; set; } = [];

    public decimal Tax { get; set; }

    public decimal Price => Items.Sum(i => i.TotalPrice);
    public decimal Discount => Items.Sum(i => i.Discount);

    public decimal Subtotal => Items.Sum(i => i.Total);
    public decimal Total => Subtotal + Tax;

    public QuoteItem? GetItemById(int quoteItemId) =>
        Items.FirstOrDefault(i => i.Id == quoteItemId);

    public QuoteItem? GetItemBy(int productId) =>
        Items.FirstOrDefault(i => i.ProductId == productId);

    public void Close() => IsOpen = false;

    public (bool, IEnumerable<ValidationResult>) Validate() =>
        QuoteValidator.Validate(this);

    public void IncludeItemsFrom(Quote other)
    {
        foreach (var otherItem in other.Items)
        {
            var matchingItem = GetItemBy(otherItem.ProductId);

            if (matchingItem != null)
                matchingItem.Quantity += otherItem.Quantity;
            else
                Items.Add(otherItem.Clone());
        }
    }

    public bool IsFree() => Total == 0;

    public bool IsFromCustomer => CustomerId.HasValue;

    public decimal GetDiscountForItem(QuoteItem item)
    {
        if (Coupon == null) return 0.0M;

        var discount = Coupon.Discount;

        if (Coupon.IsDiscountFixed) return discount * item.Quantity;

        return (item.UnitPrice * discount / 100) * item.Quantity;
    }
}
