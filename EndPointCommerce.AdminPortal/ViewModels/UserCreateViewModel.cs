using System.ComponentModel.DataAnnotations;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.AdminPortal.ViewModels;

public class UserCreateViewModel : UserViewModel
{
    [Required]
    public override string? Password { get; set; }

    public static async Task<UserCreateViewModel> CreateDefault(
        IIdentityService identityService,
        ICustomerRepository customerRepository
    ) {
        var userViewModel = new UserCreateViewModel() { Email = "" };
        await userViewModel.FillRoles(identityService);
        await userViewModel.FillCustomers(customerRepository);
        return userViewModel;
    }
}
