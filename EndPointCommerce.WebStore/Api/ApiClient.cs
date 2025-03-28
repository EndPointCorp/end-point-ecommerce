using System.Net;

namespace EndPointCommerce.WebStore.Api;

public interface IApiClient
{
    Task<List<Category>> GetCategories();
    Task<List<Product>> GetProducts();
    Task<Product> GetProduct(int id);
    Task<List<Product>> GetProductsByCategoryId(int id);
    Task<ResponseWithCookie<Quote>> GetQuote(string? quoteCookie);
    Task<ResponseWithCookie<QuoteItem>> PostQuoteItem(int productId, int quantity, string? quoteCookie);
    Task<ResponseWithCookie<QuoteItem>> PutQuoteItem(int id, int quantity, string? quoteCookie);
    Task<ResponseWithCookie<NoContent>> DeleteQuoteItem(int id, string? quoteCookie);
}

public class ResponseWithCookie<T>
{
    public T Body { get; set; } = default!;
    public string? Cookie { get; set; }
}

public class ApiClient : IApiClient, IDisposable
{
    private const string QUOTE_COOKIE_NAME = "EndPointCommerce_QuoteId";

    private readonly HttpClient _httpClient;
    private readonly Uri _baseApiUri;

    public ApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _baseApiUri = new Uri(
            config["EndPointCommerceApiUrl"] ??
                throw new InvalidOperationException("Config setting 'EndPointCommerceApiUrl' not found.")
        );
    }

    public void Dispose() => _httpClient?.Dispose();

    public async Task<List<Category>> GetCategories()
    {
        using var response = await _httpClient.GetAsync("api/Categories");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Category>>())!;
    }

    public async Task<List<Product>> GetProducts()
    {
        using var response = await _httpClient.GetAsync("api/Products");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Product>>())!;
    }

    public async Task<Product> GetProduct(int id)
    {
        using var response = await _httpClient.GetAsync($"api/Products/{id}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Product>())!;
    }

    public async Task<List<Product>> GetProductsByCategoryId(int id)
    {
        using var response = await _httpClient.GetAsync($"api/Products/CategoryId/{id}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Product>>())!;
    }

    public async Task<ResponseWithCookie<Quote>> GetQuote(string? quoteCookie)
    {
        return await WithCookie(quoteCookie, async httpClient => {
            using var response = await httpClient.GetAsync("api/Quote");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<Quote>())!;
        });
    }

    public async Task<ResponseWithCookie<QuoteItem>> PostQuoteItem(
        int productId, int quantity, string? quoteCookie
    ) {
        return await WithCookie(quoteCookie, async httpClient => {
            using var response = await httpClient.PostAsJsonAsync(
                "api/Quote/Items",
                new { ProductId = productId, Quantity = quantity }
            );
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<QuoteItem>())!;
        });
    }

    public async Task<ResponseWithCookie<QuoteItem>> PutQuoteItem(
        int id, int quantity, string? quoteCookie
    ) {
        return await WithCookie(quoteCookie, async httpClient => {
            using var response = await httpClient.PutAsJsonAsync(
                $"api/Quote/Items/{id}",
                new { Quantity = quantity }
            );
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<QuoteItem>())!;
        });
    }

    public async Task<ResponseWithCookie<NoContent>> DeleteQuoteItem(
        int id, string? quoteCookie
    ) {
        return await WithCookie(quoteCookie, async httpClient => {
            using var response = await httpClient.DeleteAsync($"api/Quote/Items/{id}");
            response.EnsureSuccessStatusCode();

            return new NoContent();
        });
    }

    private async Task<ResponseWithCookie<T>> WithCookie<T>(
        string? quoteCookie,
        Func<HttpClient, Task<T>> func
    ) {
        var cookies = new CookieContainer();

        if (quoteCookie != null)
            cookies.SetCookies(_baseApiUri, quoteCookie);

        var httpClient = new HttpClient(new HttpClientHandler { CookieContainer = cookies });
        httpClient.BaseAddress = _baseApiUri;

        var responseBody = await func(httpClient);
        var response = new ResponseWithCookie<T> { Body = responseBody };

        if (cookies.GetAllCookies().Any(c => c.Name == QUOTE_COOKIE_NAME))
            response.Cookie = cookies.GetCookieHeader(_baseApiUri);

        return response;
    }
}
