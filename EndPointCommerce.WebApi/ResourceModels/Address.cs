namespace EndPointCommerce.WebApi.ResourceModels;

public class Address
{
    public int? Id { get; set; }

    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Street { get; set; }
    public string? StreetTwo { get; set; }
    public required string City { get; set; }
    public required string ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public State? State { get; set; }
    public required int StateId { get; set; }
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
            State = State.FromEntity(entity.State),
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
        entity.StateId = StateId;
        return entity;
    }

    public static List<Address> FromListOfEntities(ICollection<Domain.Entities.Address>? entities)
    {
        if (entities != null)
            return entities.Select(FromEntity).ToList();
        return new List<Address>();
    }
}