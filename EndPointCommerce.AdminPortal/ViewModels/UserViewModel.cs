using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using EndPointCommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for users.
/// </summary>
public class UserViewModel : User
{
    [Required]
    public override string? Email { get; set; }

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public string RoleName { get; set; } = default!;

    public IEnumerable<SelectListItem>? Roles { get; set; }
    public IEnumerable<SelectListItem>? Customers { get; set; }

    public User ToModel()
    {
        return new User() {
            Id = Id,
            UserName = Email,
            Email = Email,
            CustomerId = CustomerId
        };
    }

    public async Task FillRoles(IIdentityService identityService)
    {
        var roles = await identityService.FetchAllRolesAsync();
        Roles =
            roles.
            Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Name
            }).ToList();
    }

    public async Task FillCustomers(ICustomerRepository customerRepository)
    {
        var customers = await customerRepository.FetchAllAsync();
        Customers =
            customers.
            Select(x => new SelectListItem()
            {
                Text = x.FullName,
                Value = x.Id.ToString()
            }).ToList();
    }

    public static async Task<UserViewModel> CreateDefault(
        IIdentityService identityService,
        ICustomerRepository customerRepository
    ) {
        var userViewModel = new UserViewModel() { Email = "" };
        await userViewModel.FillRoles(identityService);
        await userViewModel.FillCustomers(customerRepository);
        return userViewModel;
    }

    public static async Task<UserViewModel> FromModel(
        User model,
        IIdentityService identityService,
        ICustomerRepository customerRepository
    ) {
        var userViewModel = await CreateDefault(identityService, customerRepository);
        userViewModel.Id = model.Id;
        userViewModel.Email = model.UserName!;
        userViewModel.CustomerId = model.CustomerId;
        userViewModel.RoleName = (await identityService.GetRoleAsync(model)).Name!;
        return userViewModel;
    }
}