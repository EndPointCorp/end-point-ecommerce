namespace EndPointCommerce.WebApi.ResourceModels;

public class UserPut
{
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }

    public Domain.Entities.User UpdateEntity(Domain.Entities.User entity)
    {
        entity.Email = Email;
        entity.PhoneNumber = PhoneNumber;
        if (entity.Customer != null)
        {
            entity.Customer.Email = Email;
            entity.Customer.Name = Name;
            entity.Customer.LastName = LastName;
        }
        return entity;
    }
}