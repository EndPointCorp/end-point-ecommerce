namespace EndPointCommerce.WebApi.ResourceModels;

public class UserPost
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }

    public Domain.Entities.User ToEntity()
    {
        return new Domain.Entities.User()
        {
            Email = Email,
            UserName = Email,
            Customer = new Domain.Entities.Customer() {
                Email = Email,
                Name = Name,
                LastName = LastName,
            }
        };
    }
}