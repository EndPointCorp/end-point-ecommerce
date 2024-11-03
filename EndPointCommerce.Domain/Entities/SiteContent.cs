using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Domain.Entities;

[Index(nameof(Name), IsUnique = true)]
public class SiteContent: BaseEntity
{
    public required string Name { get; set; }
    public string? Content { get; set; }
}
