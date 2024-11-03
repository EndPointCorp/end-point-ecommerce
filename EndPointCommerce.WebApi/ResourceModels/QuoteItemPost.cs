using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.WebApi.ResourceModels;

public class QuoteItemPost
{
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}