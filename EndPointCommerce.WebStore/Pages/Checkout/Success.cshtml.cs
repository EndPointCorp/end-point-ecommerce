using System.ComponentModel.DataAnnotations;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages.Checkout;

public class SuccessModel : BasePageModel
{
    public SuccessModel(IApiClient apiClient) : base(apiClient) { }

    public int OrderId { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int orderId)
    {
        await FetchCategories();

        OrderId = orderId;

        return Page();
    }
}

