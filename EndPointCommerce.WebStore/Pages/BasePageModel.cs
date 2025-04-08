using System.Net;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    public IEnumerable<SelectListItem> States { get; set; } = [];

    public Quote? Quote { get; set; }

    [ViewData]
    public int QuoteItemCount { get; set; }

    public bool QuoteIsEmpty => Quote == null || Quote.Items.Count == 0;

    private const string OrderJsonSessionKey = "ORDER_JSON_SESSION_KEY";

    public Order? Order
    {
        get
        {
            var json = HttpContext.Session.GetString(OrderJsonSessionKey);
            if (string.IsNullOrEmpty(json)) return null;

            return System.Text.Json.JsonSerializer.Deserialize<Order>(json);
        }

        set
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            HttpContext.Session.SetString(OrderJsonSessionKey, json);
        }
    }

    [TempData]
    public string? SuccessAlertMessage { get; set; }

    protected async Task FetchCategories()
    {
        Categories = (await _apiClient.GetCategories()).OrderBy(c => c.Name).ToList();
    }

    protected async Task FetchStates()
    {
        States = (await _apiClient.GetStates())
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
    }

    protected async Task FetchQuote()
    {
        try
        {
            var response = await _apiClient.GetQuote(QuoteCookie);
            if (response.Cookie != null) QuoteCookie = response.Cookie;

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
        var response = await _apiClient.PostQuoteItem(productId, quantity, QuoteCookie);
        if (response.Cookie != null) QuoteCookie = response.Cookie;

        SuccessAlertMessage = "Product added to cart.";
    }

    private const string QuoteCookieSessionKey = "QUOTE_COOKIE_SESSION_KEY";

    protected string? QuoteCookie
    {
        get => HttpContext.Session.GetString(QuoteCookieSessionKey);
        set => HttpContext.Session.SetString(QuoteCookieSessionKey, value ?? string.Empty);
    }

    protected void ClearQuoteCookie()
    {
        HttpContext.Session.Remove(QuoteCookieSessionKey);
    }
}
