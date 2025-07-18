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
    Task<List<Order>> GetOrders();
    Task<Order> GetOrder(string guid);
    Task<List<Address>> GetAddresses();
    Task<Address> GetAddress(int id);
    Task<Address> PostAddress(Address address);
    Task<Address> PutAddress(Address address);
    Task<NoContent> DeleteAddress(int id);
    Task<User> GetUser();
    Task<HttpResponseMessage> PostUser(string email, string password, string name, string lastName);
    Task<HttpResponseMessage> PutUser(string email, string phoneNumber, string name, string lastName);
    Task<HttpResponseMessage> PostUserLogin(string email, string password);
    Task<HttpResponseMessage> PostUserLogout();
    Task<HttpResponseMessage> GetUserConfirmEmail(string userId, string code);
    Task<HttpResponseMessage> PostUserForgotPassword(string email);
    Task<HttpResponseMessage> PostUserResetPassword(string email, string resetCode, string newPassword);
    Task<HttpResponseMessage> PostUserManageInfo(string oldPassword, string newPassword);
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

    public async Task<List<Order>> GetOrders()
    {
        using var response = await _httpClient.GetAsync("api/Orders");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Order>>())!;
    }

    public async Task<Order> GetOrder(string guid)
    {
        using var response = await _httpClient.GetAsync($"api/Orders/{guid}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Order>())!;
    }

    public async Task<List<Address>> GetAddresses()
    {
        using var response = await _httpClient.GetAsync("api/Addresses");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Address>>())!;
    }

    public async Task<Address> GetAddress(int id)
    {
        using var response = await _httpClient.GetAsync($"api/Addresses/{id}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Address>())!;
    }

    public async Task<Address> PostAddress(Address address)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/Addresses", address);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Address>())!;
    }

    public async Task<Address> PutAddress(Address address)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/Addresses/{address.Id}", address);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Address>())!;
    }

    public async Task<NoContent> DeleteAddress(int id)
    {
        using var response = await _httpClient.DeleteAsync($"api/Addresses/{id}");
        response.EnsureSuccessStatusCode();

        return new NoContent();
    }

    public async Task<User> GetUser()
    {
        using var response = await _httpClient.GetAsync("api/User");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<User>())!;
    }

    public async Task<HttpResponseMessage> PostUser(string email, string password, string name, string lastName)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/User",
            new { email, password, name, lastName }
        );

        return response;
    }

    public async Task<HttpResponseMessage> PutUser(string email, string phoneNumber, string name, string lastName)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "api/User",
            new { email, phoneNumber, name, lastName }
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

    public async Task<HttpResponseMessage> GetUserConfirmEmail(string userId, string code)
    {
        var response = await _httpClient.GetAsync(
            $"/api/User/confirmEmail?userId={userId}&code={code}"
        );
        response.EnsureSuccessStatusCode();

        return response;
    }

    public async Task<HttpResponseMessage> PostUserForgotPassword(string email)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/User/forgotPassword",
            new { email }
        );
        response.EnsureSuccessStatusCode();

        return response;
    }

    public async Task<HttpResponseMessage> PostUserResetPassword(string email, string resetCode, string newPassword)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/User/resetPassword",
            new { email, resetCode, newPassword }
        );

        return response;
    }

    public async Task<HttpResponseMessage> PostUserManageInfo(string oldPassword, string newPassword)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/User/manage/info",
            new { oldPassword, newPassword }
        );

        return response;
    }
}
