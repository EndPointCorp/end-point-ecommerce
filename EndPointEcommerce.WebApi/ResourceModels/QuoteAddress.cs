using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class QuoteAddress
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
    public int CountryId { get; set; }
    public int? StateId { get; set; }
    public State? State { get; set; }
    [Required]
    public string ZipCode { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public Domain.Entities.Address ToEntity()
    {
        return new()
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

    public static QuoteAddress FromEntity(Domain.Entities.Address entity)
    {
        return new()
        {
            Id = entity.Id,

            Name = entity.Name,
            LastName = entity.LastName,
            Street = entity.Street,
            StreetTwo = entity.StreetTwo,
            City = entity.City,
            CountryId = entity.CountryId,
            StateId = entity.StateId,
            State = entity.State != null ? State.FromEntity(entity.State) : null,
            ZipCode = entity.ZipCode,
            PhoneNumber = entity.PhoneNumber
        };
    }

    public static List<QuoteAddress> FromListOfEntities(IList<Domain.Entities.Address> entities) =>
        entities.Select(FromEntity).ToList();
}
