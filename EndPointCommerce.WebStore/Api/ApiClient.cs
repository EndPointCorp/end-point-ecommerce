namespace EndPointCommerce.WebStore.Api;

public interface IApiClient
{
    Task<List<Product>> GetProducts();
}

public class ApiClient : IApiClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public void Dispose() => _httpClient?.Dispose();

    public async Task<List<Product>> GetProducts()
    {
        using var response = await _httpClient.GetAsync("api/Products");
        response.EnsureSuccessStatusCode();

        _logger.LogInformation($"--> {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}");
        _logger.LogInformation($"<-- {(int)response.StatusCode} {response.ReasonPhrase}");
        _logger.LogDebug($"<-- {await response.Content.ReadAsStringAsync()}");

        return (await response.Content.ReadFromJsonAsync<List<Product>>())!;
    }
}
