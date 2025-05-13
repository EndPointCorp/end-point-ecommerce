using System.ComponentModel.DataAnnotations;
using FoolProof.Core;

namespace EndPointCommerce.Domain.Entities;

/// <summary>
/// Entity for addresses.
/// </summary>
public class Address : BaseAuditEntity
{
    [Display(Name = "First Name")]
    public required string Name { get; set; }
    [Display(Name = "Last Name")]
    public required string LastName { get; set; }
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Street Line 1")]
    public required string Street { get; set; }
    [Display(Name = "Street Line 2")]
    public string? StreetTwo { get; set; }
    public required string City { get; set; }
    [Display(Name = "Zip Code")]
    public required string ZipCode { get; set; }

    public Country? Country { get; set; } = default!;
    [Display(Name = "Country")]
    public required int CountryId { get; set; }

    public State? State { get; set; } = default!;
    [Display(Name = "State")]
    [RequiredIfTrue("CountryIsUs", ErrorMessage = "State is required for US addresses.")]
    public int? StateId { get; set; }

    public Customer? Customer { get; set; } = default!;
    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    public bool CountryIsUs => CountryId == Country.US_COUNTRY_ID;

    [Display(Name = "Address")]
    public string FullAddress
    {
        get
        {
            var address = Street;
            if (!string.IsNullOrEmpty(StreetTwo)) address += $", {StreetTwo}";
            address += $", {City}";
            if (State != null) address += $", {State.Name}";
            address += $", {ZipCode}";
            if (Country != null) address += $", {Country.Name}";

            return address;
        }
    }

    public string FullName => $"{Name} {LastName}";

    public Address Clone() =>
        new()
        {
            Name = Name,
            LastName = LastName,
            PhoneNumber = PhoneNumber,
            Street = Street,
            StreetTwo = StreetTwo,
            City = City,
            ZipCode = ZipCode,
            State = State,
            StateId = StateId,
            Country = Country,
            CountryId = CountryId
        };
}
