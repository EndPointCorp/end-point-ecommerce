using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class CartModel : BasePageModel
{
    public CartModel(IApiClient apiClient) : base(apiClient) { }

    public async Task OnGetAsync()
    {
        await FetchCategories();
        await FetchQuote();
    }

    public async Task<IActionResult> OnPostUpdateItemAsync(int itemId, int quantity)
    {
        var response = await _apiClient.PutQuoteItem(itemId, quantity, QuoteCookie);
        if (response.Cookie != null) QuoteCookie = response.Cookie;

        SuccessAlertMessage = "Item updated.";

        return RedirectToPage("/Cart");
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int itemId)
    {
        var response = await _apiClient.DeleteQuoteItem(itemId, QuoteCookie);
        if (response.Cookie != null) QuoteCookie = response.Cookie;

        SuccessAlertMessage = "Item removed.";

        return RedirectToPage("/Cart");
    }
}

