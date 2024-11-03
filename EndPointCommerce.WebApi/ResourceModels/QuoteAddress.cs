namespace EndPointCommerce.WebApi.ResourceModels;

public class QuoteAddress
{
    public int? Id { get; set; }

    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Street { get; set; }
    public string? StreetTwo { get; set; }
    public required string City { get; set; }
    public required int StateId { get; set; }
    public required string ZipCode { get; set; }
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
            StateId = entity.StateId,
            ZipCode = entity.ZipCode,
            PhoneNumber = entity.PhoneNumber
        };
    }

    public static List<QuoteAddress> FromListOfEntities(IList<Domain.Entities.Address> entities) =>
        entities.Select(FromEntity).ToList();
}
