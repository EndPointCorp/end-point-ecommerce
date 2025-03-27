using System.Net;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointCommerce.WebStore.Pages;

public abstract class PageWithQuoteModel : PageModel
{
    protected readonly IApiClient _apiClient;

    public PageWithQuoteModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [ViewData]
    public List<Category> Categories { get; set; } = [];

    public Quote? Quote { get; set; }

    [ViewData]
    public int QuoteItemCount { get; set; }

    protected async Task FetchCategories()
    {
        Categories = (await _apiClient.GetCategories()).OrderBy(c => c.Name).ToList();
    }

    protected async Task FetchQuote()
    {
        try
        {
            var quote = await _apiClient.GetQuote(GetQuoteCookie());
            if (quote.Cookie != null) SetQuoteCookie(quote.Cookie);

            Quote = quote.Body;
            QuoteItemCount = Quote?.Items.Count ?? 0;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode != HttpStatusCode.NotFound) throw;
        }
    }

    private const string QuoteCookieSessionKey = "QUOTE_COOKIE_SESSION_KEY";

    protected string? GetQuoteCookie() =>
        HttpContext.Session.GetString(QuoteCookieSessionKey);

    protected void SetQuoteCookie(string quoteCookie) =>
        HttpContext.Session.SetString(QuoteCookieSessionKey, quoteCookie);
}
