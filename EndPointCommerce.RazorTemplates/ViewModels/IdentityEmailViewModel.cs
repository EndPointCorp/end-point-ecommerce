using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.RazorTemplates.ViewModels;

public class IdentityEmailViewModel
{
    public required User User { get; set; }
    public required string Link { get; set; }
}
