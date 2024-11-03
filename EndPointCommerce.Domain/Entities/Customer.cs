using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.Domain.Entities;

/// <summary>
/// Entity for customers.
/// </summary>
public class Customer: BaseAuditEntity
{
    [Display(Name = "First Name")]
    public required string Name { get; set; }
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }
    [Display(Name = "Email")]
    public required string Email { get; set; }
    [Display(Name = "Full Name")]
    public string FullName => LastName == null ? Name : $"{Name} {LastName}";
}