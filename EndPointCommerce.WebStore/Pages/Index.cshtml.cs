using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class IndexModel : BasePageModel
{
    public IndexModel(IApiClient apiClient) : base(apiClient) { }

    public List<Product> Products { get; set; } = [];

    public async Task OnGetAsync()
    {
        await FetchCategories();
        await FetchQuote();
        Products = await _apiClient.GetProducts();
    }

    public async Task<IActionResult> OnPostAddToQuoteAsync(int productId)
    {
        await AddItemToQuote(productId, 1);

        return RedirectToPage("/Index");
    }
}
