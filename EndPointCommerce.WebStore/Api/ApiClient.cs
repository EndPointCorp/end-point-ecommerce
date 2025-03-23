namespace EndPointCommerce.WebStore.Api;

public interface IApiClient
{
    Task<List<Product>> GetProducts();
    Task<Product> GetProduct(int id);
}

public class ApiClient : IApiClient, IDisposable
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Dispose() => _httpClient?.Dispose();

    public async Task<List<Product>> GetProducts()
    {
        using var response = await _httpClient.GetAsync("api/Products");
        response.EnsureSuccessStatusCode();

        // Console.WriteLine($"--> {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}");
        // Console.WriteLine($"<-- {(int)response.StatusCode} {response.ReasonPhrase}");
        // Console.WriteLine($"<-- {await response.Content.ReadAsStringAsync()}");

        return (await response.Content.ReadFromJsonAsync<List<Product>>())!;
    }

    public async Task<Product> GetProduct(int id)
    {
        using var response = await _httpClient.GetAsync($"api/Products/{id}");
        response.EnsureSuccessStatusCode();

        // Console.WriteLine($"--> {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}");
        // Console.WriteLine($"<-- {(int)response.StatusCode} {response.ReasonPhrase}");
        // Console.WriteLine($"<-- {await response.Content.ReadAsStringAsync()}");

        return (await response.Content.ReadFromJsonAsync<Product>())!;
    }
}
