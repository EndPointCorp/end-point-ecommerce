namespace EndPointCommerce.WebApi.ResourceModels;

public class State
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public static State FromEntity(Domain.Entities.State entity)
    {
        return new() {
            Id = entity.Id,
            Name = entity.Name,
        };
    }

    public static List<State> FromListOfEntities(ICollection<Domain.Entities.State> entities) =>
        entities.Select(FromEntity).ToList();
}