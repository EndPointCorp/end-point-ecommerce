using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class QuoteItemPut
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}