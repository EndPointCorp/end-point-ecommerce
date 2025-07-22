using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.WebApi.ResourceModels;

public class UserPut
{
    [Required]
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;

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
