using System.ComponentModel.DataAnnotations;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class ProductModel : BasePageModel
{
    public ProductModel(IApiClient apiClient) : base(apiClient) { }

    public Product Product { get; set; } = default!;

    [BindProperty]
    [Required, Range(1, int.MaxValue)]
    public int ProductQuantity { get; set; } = 1;

    public async Task OnGetAsync(int id)
    {
        await LoadData(id);
    }

    public async Task<IActionResult> OnPostAddToQuoteAsync(int productId)
    {
        if (!ModelState.IsValid)
        {
            await LoadData(productId);
            return Page();
        }

        await AddItemToQuote(productId, ProductQuantity);

        return RedirectToPage("/Product", new { Id = productId });
    }

    private async Task LoadData(int productId)
    {
        await FetchCategories();
        await FetchQuote();
        Product = await _apiClient.GetProduct(productId);
    }
}