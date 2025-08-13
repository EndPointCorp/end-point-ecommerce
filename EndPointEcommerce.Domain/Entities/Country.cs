using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity for countries.
/// </summary>
[Index(nameof(Code), IsUnique = true)]
public class Country : BaseEntity
{
    public const int US_COUNTRY_ID = 225;

    [Display(Name = "Country")]
    public required string Name { get; set; }
    public required string Code { get; set; }

    [Display(Name = "Enabled")]
    public bool IsEnabled { get; set; } = true;
}