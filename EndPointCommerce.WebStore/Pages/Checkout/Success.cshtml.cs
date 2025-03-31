using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages.Checkout;

public class SuccessModel : BasePageModel
{
    public SuccessModel(IApiClient apiClient) : base(apiClient) { }

    public string? ShippingAddressState { get; set; }
    public string? BillingAddressState { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (Order == null) return RedirectToPage("/Cart");

        await FetchCategories();
        await FetchStates();

        ShippingAddressState = States.FirstOrDefault(s => s.Value == Order.ShippingAddress.StateId.ToString())?.Text;
        BillingAddressState = States.FirstOrDefault(s => s.Value == Order.BillingAddress.StateId.ToString())?.Text;

        return Page();
    }
}

