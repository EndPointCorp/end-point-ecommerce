using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointCommerce.WebStore.Pages;

public class IndexModel : PageModel
{
    private readonly IApiClient _apiClient;
    private readonly string _authNetEnvironment = string.Empty;

    public IndexModel(IApiClient apiClient, IConfiguration config)
    {
        _apiClient = apiClient;

        _authNetEnvironment = config["AuthNetEnvironment"]!;

        EndPointCommerceApiUrl = config["EndPointCommerceApiUrl"]!;
        AuthNetLoginId = config["AuthNetLoginId"]!;
        AuthNetClientKey = config["AuthNetClientKey"]!;
    }

    public string EndPointCommerceApiUrl { get; set; }
    public string AuthNetLoginId { get; set; }
    public string AuthNetClientKey { get; set; }

    public string AcceptJsUrl => _authNetEnvironment == "Production" ?
        "https://js.authorize.net/v1/Accept.js" :
        "https://jstest.authorize.net/v1/Accept.js";

    public async Task OnGetAsync()
    {
        var products = await _apiClient.GetProducts();
    }
}