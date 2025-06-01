namespace EndPointCommerce.WebApi.ResourceModels;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Customer? Customer { get; set; }

    public static User FromEntity(Domain.Entities.User entity)
    {
        return new() {
            Id = entity.Id,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email!,
            Customer = entity.Customer != null ? Customer.FromEntity(entity.Customer) : null
        };
    }
}