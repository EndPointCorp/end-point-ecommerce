using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.WebStoreSpa.Api;

public class Address
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Street { get; set; } = string.Empty;

    public string? StreetTwo { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    private const int US_COUNTRY_ID = 225;
    public int CountryId { get; set; } = US_COUNTRY_ID;

    [Required]
    public int StateId { get; set; } = 1;
    public State? State { get; set; }

    [Required]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip Code format.")]
    public string ZipCode { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid Phone Number format.")]
    public string? PhoneNumber { get; set; }

    public string FullName => $"{Name} {LastName}";

    public Address Clone()
    {
        return new Address
        {
            Name = Name,
            LastName = LastName,
            Street = Street,
            StreetTwo = StreetTwo,
            City = City,
            CountryId = CountryId,
            StateId = StateId,
            ZipCode = ZipCode,
            PhoneNumber = PhoneNumber
        };
    }
}
