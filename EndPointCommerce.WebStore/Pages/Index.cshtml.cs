using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointCommerce.WebStore.Pages;

public class IndexModel : PageModel
{
    private readonly string _authNetEnvironment = string.Empty;

    public IndexModel(IConfiguration config)
    {
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

    public void OnGet() { }
}
