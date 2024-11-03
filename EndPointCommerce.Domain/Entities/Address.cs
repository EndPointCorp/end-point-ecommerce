using System.ComponentModel.DataAnnotations;

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

    public State State { get; set; } = default!;
    [Display(Name = "State")]
    public required int StateId { get; set; }

    public Customer? Customer { get; set; } = default!;
    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    [Display(Name = "Address")]
    public string FullAddress => $"{Street}, {City}, {State.Name}, {ZipCode}";

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
            StateId = StateId
        };
}
