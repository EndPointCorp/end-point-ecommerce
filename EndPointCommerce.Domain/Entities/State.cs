using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.Domain.Entities;

/// <summary>
/// Entity for states.
/// </summary>
[Index(nameof(Abbreviation), IsUnique = true)]
public class State : BaseEntity
{
    [Display(Name = "State")]
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
}