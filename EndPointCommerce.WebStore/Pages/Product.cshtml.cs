using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointCommerce.WebStore.Pages;

public class ProductModel : PageModel
{
    private readonly IApiClient _apiClient;

    public ProductModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task OnGetAsync(int id)
    {
        var product = await _apiClient.GetProduct(id);
    }
}