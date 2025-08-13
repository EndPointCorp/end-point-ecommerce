using System.Net;
using EndPointEcommerce.WebStore.Api;

namespace EndPointEcommerce.WebStore.State;

public class QuoteStateContainer
{
    private readonly IApiClient _apiClient;

    public QuoteStateContainer(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        _apiClient = scope.ServiceProvider.GetRequiredService<IApiClient>();
    }

    public Quote? Quote { get; set; }

    public bool IsQuoteEmpty => Quote == null || Quote.Items.Count == 0;

    public int QuoteItemCount => Quote?.Items.Count ?? 0;

    public event Action? QuoteUpdated;

    public async Task LoadQuote()
    {
        try
        {
            Quote = await _apiClient.GetQuote();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            Quote = null;
        }

        QuoteUpdated?.Invoke();
    }
}
