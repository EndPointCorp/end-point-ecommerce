using System.ComponentModel.DataAnnotations;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages;

public class ProductModel : PageWithQuoteModel
{
    public ProductModel(IApiClient apiClient) : base(apiClient) { }

    public Product Product { get; set; } = default!;

    private readonly IApiClient _apiClient;

    public ProductModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task OnGetAsync(int id)
    {
        await LoadData(id);
    }

    private async Task LoadData(int productId)
    {
        await FetchQuote();
        Product = await _apiClient.GetProduct(productId);
    }
}