namespace EndPointCommerce.WebApi.ResourceModels;

public class State
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;

    public static State FromEntity(Domain.Entities.State entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Abbreviation = entity.Abbreviation
        };
    }

    public static List<State> FromListOfEntities(ICollection<Domain.Entities.State> entities) =>
        entities.Select(FromEntity).ToList();
}
