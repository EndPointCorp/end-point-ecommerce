using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class CategoryModel : PageWithQuoteModel
{
    public CategoryModel(IApiClient apiClient) : base(apiClient) { }

    public Category Category { get; set; } = default!;
    public List<Product> Products { get; set; } = [];

    [TempData]
    public string? SuccessAlertMessage { get; set; }

    public async Task OnGetAsync(int id)
    {
        await FetchCategories();
        await FetchQuote();
        Category = Categories.Single(c => c.Id == id);
        Products = await _apiClient.GetProductsByCategoryId(id);
    }

    public async Task<IActionResult> OnPostAddToQuoteAsync(int productId, int categoryId)
    {
        var quoteItem = await _apiClient.PostQuoteItem(productId, 1, GetQuoteCookie());
        if (quoteItem.Cookie != null) SetQuoteCookie(quoteItem.Cookie);

        SuccessAlertMessage = "Product added to cart";

        return RedirectToPage("/Category", new { Id = categoryId });
    }
}