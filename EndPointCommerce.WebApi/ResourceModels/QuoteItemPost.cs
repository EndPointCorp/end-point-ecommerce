using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.WebApi.ResourceModels;

public class QuoteItemPost
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}