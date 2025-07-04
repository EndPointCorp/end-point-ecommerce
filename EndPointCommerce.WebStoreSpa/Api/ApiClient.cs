using System.Net;
using System.Net.Http.Json;

namespace EndPointCommerce.WebStoreSpa.Api;

public interface IApiClient
{
    Task<List<Country>> GetCountries();
    Task<List<State>> GetStates();
    Task<List<Category>> GetCategories();
    Task<List<Product>> GetProducts();
    Task<Product> GetProduct(int id);
    Task<List<Product>> GetProductsByCategoryId(int id);
    Task<Quote> GetQuote();
    Task<NoContent> PutQuote(string email, Address shippingAddress, Address billingAddress);
    // Task<ResponseWithCookie<Quote>> PostQuoteValidate(string? quoteCookie);
    Task<QuoteItem> PostQuoteItem(int productId, int quantity);
    Task<QuoteItem> PutQuoteItem(int id, int quantity);
    Task<NoContent> DeleteQuoteItem(int id);
    Task<Order> PostOrder(string paymentMethodNonceValue, string paymentMethodNonceDescriptor);
}

// public class ResponseWithCookie<T>
// {
//     public T Body { get; set; } = default!;
//     public string? Cookie { get; set; }
// }

public class ApiClient : IApiClient
{
    // private const string QUOTE_COOKIE_NAME = "EndPointCommerce_QuoteId";

    private readonly HttpClient _httpClient;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("EndPointCommerce.WebApi");
    }

    public async Task<List<Country>> GetCountries()
    {
        using var response = await _httpClient.GetAsync("api/Countries");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Country>>())!;
    }

    public async Task<List<State>> GetStates()
    {
        using var response = await _httpClient.GetAsync("api/States");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<State>>())!;
    }

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

    public async Task<Quote> GetQuote()
    {
        using var response = await _httpClient.GetAsync("api/Quote");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Quote>())!;
    }

    public async Task<NoContent> PutQuote(string email, Address shippingAddress, Address billingAddress)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            "api/Quote",
            new { Email = email, ShippingAddress = shippingAddress, BillingAddress = billingAddress }
        );
        response.EnsureSuccessStatusCode();

        return new NoContent();
    }

    // public async Task<ResponseWithCookie<Quote>> PostQuoteValidate(string? quoteCookie)
    // {
    //     return await WithCookie(quoteCookie, async httpClient => {
    //         using var response = await httpClient.PostAsJsonAsync("api/Quote/Validate", new { });
    //         response.EnsureSuccessStatusCode();

    //         return (await response.Content.ReadFromJsonAsync<Quote>())!;
    //     });
    // }

    public async Task<QuoteItem> PostQuoteItem(int productId, int quantity)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/Quote/Items",
            new { ProductId = productId, Quantity = quantity }
        );
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<QuoteItem>())!;
    }

    public async Task<QuoteItem> PutQuoteItem(int id, int quantity)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/Quote/Items/{id}",
            new { Quantity = quantity }
        );
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<QuoteItem>())!;
    }

    public async Task<NoContent> DeleteQuoteItem(int id)
    {
        using var response = await _httpClient.DeleteAsync($"api/Quote/Items/{id}");
        response.EnsureSuccessStatusCode();

        return new NoContent();
    }

    public async Task<Order> PostOrder(string paymentMethodNonceValue, string paymentMethodNonceDescriptor)
    {
        return await Task.FromResult(
            new Order(
                Id: 123456,
                Status: "test_order_status",
                TrackingNumber: "123456789",
                Total: 100.00m,
                Items: null!,
                ShippingAddress: null!,
                BillingAddress: null!
            )
        );

        // using var response = await _httpClient.PostAsJsonAsync(
        //     "api/Orders",
        //     new
        //     {
        //         PaymentMethodNonceValue = paymentMethodNonceValue,
        //         PaymentMethodNonceDescriptor = paymentMethodNonceDescriptor
        //     }
        // );
        // response.EnsureSuccessStatusCode();

        // return (await response.Content.ReadFromJsonAsync<Order>())!;
    }

    // private async Task<ResponseWithCookie<T>> WithCookie<T>(
    //     string? quoteCookie,
    //     Func<HttpClient, Task<T>> func
    // ) {
    //     var cookies = new CookieContainer();

    //     if (quoteCookie != null)
    //         cookies.SetCookies(_baseApiUri, quoteCookie);

    //     var httpClient = new HttpClient(new HttpClientHandler { CookieContainer = cookies });
    //     httpClient.BaseAddress = _baseApiUri;

    //     var responseBody = await func(httpClient);
    //     var response = new ResponseWithCookie<T> { Body = responseBody };

    //     if (cookies.GetAllCookies().Any(c => c.Name == QUOTE_COOKIE_NAME))
    //         response.Cookie = cookies.GetCookieHeader(_baseApiUri);

    //     return response;
    // }
}
