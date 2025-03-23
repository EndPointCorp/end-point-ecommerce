using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointCommerce.WebStore.Pages;

public class IndexModel : PageModel
{
    private readonly IApiClient _apiClient;

    public IndexModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task OnGetAsync()
    {
        var products = await _apiClient.GetProducts();
    }
}