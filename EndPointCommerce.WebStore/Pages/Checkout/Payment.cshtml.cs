using System.ComponentModel.DataAnnotations;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages.Checkout;

public class PaymentModel : BasePageModel
{
    private readonly string _authNetEnvironment = string.Empty;

    public PaymentModel(IApiClient apiClient, IConfiguration config) : base(apiClient)
    {
        _authNetEnvironment = config["AuthNetEnvironment"]!;

        AuthNetLoginId = config["AuthNetLoginId"]!;
        AuthNetClientKey = config["AuthNetClientKey"]!;
    }

    public string AuthNetLoginId { get; set; }
    public string AuthNetClientKey { get; set; }

    public string AcceptJsUrl => _authNetEnvironment == "Production" ?
        "https://js.authorize.net/v1/Accept.js" :
        "https://jstest.authorize.net/v1/Accept.js";

    [BindProperty]
    [Required]
    public string PaymentMethodNonceValue { get; set; } = default!;

    [BindProperty]
    [Required]
    public string PaymentMethodNonceDescriptor { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        await FetchCategories();
        await FetchQuote();

        if (QuoteIsEmpty) return RedirectToPage("/Cart");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await FetchQuote();
        if (QuoteIsEmpty) return RedirectToPage("/Cart");

        if (!ModelState.IsValid)
        {
            await FetchCategories();
            return Page();
        }

        try
        {
            await _apiClient.PostQuoteValidate(GetQuoteCookie());
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Cart is not valid. Please try again.");

            await FetchCategories();
            return Page();
        }

        var response = await _apiClient.PostOrder(
            PaymentMethodNonceValue,
            PaymentMethodNonceDescriptor,
            GetQuoteCookie()
        );

        return RedirectToPage("/Checkout/Success", new { orderId = response.Body.Id });
    }
}
