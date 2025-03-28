using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class CategoryModel : BasePageModel
{
    public CategoryModel(IApiClient apiClient) : base(apiClient) { }

    public Category Category { get; set; } = default!;
    public List<Product> Products { get; set; } = [];

    public async Task OnGetAsync(int id)
    {
        await FetchCategories();
        await FetchQuote();
        Category = Categories.Single(c => c.Id == id);
        Products = await _apiClient.GetProductsByCategoryId(id);
    }

    public async Task<IActionResult> OnPostAddToQuoteAsync(int productId, int categoryId)
    {
        await AddItemToQuote(productId, 1);

        return RedirectToPage("/Category", new { Id = categoryId });
    }
}