namespace EndPointEcommerce.WebApi.ResourceModels;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName => $"{Name} {LastName}";

    public static Customer FromEntity(Domain.Entities.Customer entity)
    {
        return new() {
            Id = entity.Id,
            Name = entity.Name,
            LastName = entity.LastName,
            Email = entity.Email,
        };
    }
}