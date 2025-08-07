using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.AdminPortal.ViewModels;

public class UserEditViewModel : UserViewModel
{
    public override string? Password { get; set; }

    public static async Task<UserEditViewModel> FromModel(
        User model,
        IIdentityService identityService,
        ICustomerRepository customerRepository
    ) {
        var userViewModel = new UserEditViewModel
        {
            Id = model.Id,
            Email = model.UserName!,
            CustomerId = model.CustomerId,
            RoleName = (await identityService.GetRoleAsync(model)).Name!
        };

        await userViewModel.FillRoles(identityService);
        await userViewModel.FillCustomers(customerRepository);

        return userViewModel;
    }
}
