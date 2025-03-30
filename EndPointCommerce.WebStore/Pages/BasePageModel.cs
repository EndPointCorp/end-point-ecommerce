using System.Net;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointCommerce.WebStore.Pages;

public abstract class BasePageModel : PageModel
{
    protected readonly IApiClient _apiClient;

    public BasePageModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [ViewData]
    public List<Category> Categories { get; set; } = [];

    public Quote? Quote { get; set; }

    [ViewData]
    public int QuoteItemCount { get; set; }

    public bool QuoteIsEmpty => Quote == null || Quote.Items.Count == 0;

    [TempData]
    public string? SuccessAlertMessage { get; set; }

    protected async Task FetchCategories()
    {
        Categories = (await _apiClient.GetCategories()).OrderBy(c => c.Name).ToList();
    }

    protected async Task FetchQuote()
    {
        try
        {
            var response = await _apiClient.GetQuote(GetQuoteCookie());
            if (response.Cookie != null) SetQuoteCookie(response.Cookie);

            Quote = response.Body;
            QuoteItemCount = Quote?.Items.Count ?? 0;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode != HttpStatusCode.NotFound) throw;
        }
    }

    protected async Task AddItemToQuote(int productId, int quantity)
    {
        var response = await _apiClient.PostQuoteItem(productId, quantity, GetQuoteCookie());
        if (response.Cookie != null) SetQuoteCookie(response.Cookie);

        SuccessAlertMessage = "Product added to cart.";
    }

    private const string QuoteCookieSessionKey = "QUOTE_COOKIE_SESSION_KEY";

    protected string? GetQuoteCookie() =>
        HttpContext.Session.GetString(QuoteCookieSessionKey);

    protected void SetQuoteCookie(string quoteCookie) =>
        HttpContext.Session.SetString(QuoteCookieSessionKey, quoteCookie);
}
