using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using EndPointCommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.AdminPortal.ViewModels;

public abstract class UserViewModel : User
{
    [Required]
    public override string? Email { get; set; }

    public abstract string? Password { get; set; }

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
}
