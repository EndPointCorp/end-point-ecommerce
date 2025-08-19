// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.AdminPortal.ViewModels;

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
