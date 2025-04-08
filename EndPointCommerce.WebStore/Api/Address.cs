using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.WebStore.Api;

public class Address
{
    [Required]
    [Display(Name = "First Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Street (line 1)")]
    public string Street { get; set; } = string.Empty;

    [Display(Name = "Street (line 2)")]
    public string? StreetTwo { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    [Display(Name = "State")]
    public int StateId { get; set; }

    [Required]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip Code format.")]
    [Display(Name = "Zip Code")]
    public string ZipCode { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid Phone Number format.")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}
