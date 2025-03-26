using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class IndexModel : PageWithQuoteModel
{
    public IndexModel(IApiClient apiClient) : base(apiClient) { }

    public List<Product> Products { get; set; } = [];

        _apiClient = apiClient;
    }

    public async Task OnGetAsync()
    {
        await FetchQuote();
        Products = await _apiClient.GetProducts();
    }

    public async Task<IActionResult> OnPostAddToQuoteAsync(int productId)
    {
        var quoteItem = await _apiClient.PostQuoteItem(productId, 1, GetQuoteCookie());
        if (quoteItem.Cookie != null) SetQuoteCookie(quoteItem.Cookie);

        SuccessAlertMessage = "Product added to cart";

        return RedirectToPage("/Index");
    }
}
