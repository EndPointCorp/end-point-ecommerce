using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for quotes.
/// </summary>
public class QuoteViewModel : Quote
{
    public static QuoteViewModel FromModel(Quote model)
    {
        return new() {
            Id = model.Id,
            Customer = model.Customer,
            Coupon = model.Coupon,
            Email = model.Email,
            IsOpen = model.IsOpen,
            ShippingAddress = model.ShippingAddress,
            BillingAddress = model.BillingAddress,
            Tax = model.Tax,
            Items = model.Items.ToList()
        };
    }
}
