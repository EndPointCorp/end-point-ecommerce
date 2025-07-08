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
    Task<QuoteItem> PostQuoteItem(int productId, int quantity);
    Task<QuoteItem> PutQuoteItem(int id, int quantity);
    Task<NoContent> DeleteQuoteItem(int id);
    Task<Order> PostOrder(string paymentMethodNonceValue, string paymentMethodNonceDescriptor);
    Task<Order> GetOrder(string guid);
    Task<HttpResponseMessage> PostUser(string email, string password, string name, string lastName);
    Task<HttpResponseMessage> PostUserLogin(string email, string password);
    Task<HttpResponseMessage> PostUserLogout();
}

public class ApiClient : IApiClient
{
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
        using var response = await _httpClient.PostAsJsonAsync(
            "api/Orders",
            new
            {
                PaymentMethodNonceValue = paymentMethodNonceValue,
                PaymentMethodNonceDescriptor = paymentMethodNonceDescriptor
            }
        );
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Order>())!;
    }

    public async Task<Order> GetOrder(string guid)
    {
        using var response = await _httpClient.GetAsync($"api/Orders/{guid}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Order>())!;
    }

    public async Task<HttpResponseMessage> PostUser(string email, string password, string name, string lastName)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/User",
            new { email, password, name, lastName }
        );

        return response;
    }

    public async Task<HttpResponseMessage> PostUserLogin(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/User/login?useCookies=true",
            new { email, password }
        );
        response.EnsureSuccessStatusCode();

        return response;
    }

    public async Task<HttpResponseMessage> PostUserLogout()
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/User/Logout",
            new { }
        );
        response.EnsureSuccessStatusCode();

        return response;
    }
}
