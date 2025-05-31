namespace EndPointCommerce.WebApi.ResourceModels;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public static Country FromEntity(Domain.Entities.Country entity)
    {
        return new() {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
        };
    }

    public static List<Country> FromListOfEntities(ICollection<Domain.Entities.Country> entities) =>
        entities.Select(FromEntity).ToList();
}