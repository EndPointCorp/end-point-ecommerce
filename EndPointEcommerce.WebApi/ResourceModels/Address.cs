using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class Address
{
    public int? Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Street { get; set; } = string.Empty;
    public string? StreetTwo { get; set; }
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string ZipCode { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Country? Country { get; set; }
    [Required]
    public int CountryId { get; set; }
    public State? State { get; set; }
    public int? StateId { get; set; }
    public string FullAddress => $"{Street}, {City}, {State?.Name}, {ZipCode}";

    public static Address FromEntity(Domain.Entities.Address entity)
    {
        return new() {
            Name = entity.Name,
            LastName = entity.LastName,
            Street = entity.Street,
            StreetTwo = entity.StreetTwo,
            City = entity.City,
            ZipCode = entity.ZipCode,
            PhoneNumber = entity.PhoneNumber,
            Country = entity.Country != null ? Country.FromEntity(entity.Country) : null,
            CountryId = entity.CountryId,
            State = entity.State != null ? State.FromEntity(entity.State) : null,
            StateId = entity.StateId,
            Id = entity.Id,
        };
    }

    public Domain.Entities.Address ToEntity()
    {
        return new()
        {
            Name = Name,
            LastName = LastName,
            Street = Street,
            StreetTwo = StreetTwo,
            City = City,
            ZipCode = ZipCode,
            PhoneNumber = PhoneNumber,
            CountryId = CountryId,
            StateId = StateId,
        };
    }

    public Domain.Entities.Address UpdateEntity(Domain.Entities.Address entity)
    {
        entity.Name = Name;
        entity.LastName = LastName;
        entity.Street = Street;
        entity.StreetTwo = StreetTwo;
        entity.City = City;
        entity.ZipCode = ZipCode;
        entity.PhoneNumber = PhoneNumber;
        entity.CountryId = CountryId;
        entity.StateId = StateId;
        return entity;
    }

    public static List<Address> FromListOfEntities(ICollection<Domain.Entities.Address> entities) =>
        entities.Select(FromEntity).ToList();
}