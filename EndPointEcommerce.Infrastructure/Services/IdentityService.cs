// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Text;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly EndPointEcommerceDbContext _dbContext;

    public IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole<int>> roleManager,
        EndPointEcommerceDbContext dbContext
    ) {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<bool> Exists(int id)
    {
        return await _dbContext.Users.FindAsync(id) != null;
    }

    public async Task<User?> FindByIdAsync(int id)
    {
        return await _dbContext.Users
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<User?> FindByUserNameAsync(string userName)
    {
        return await _dbContext.Users
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.UserName!.ToLower().Equals(userName.ToLower()));
    }

    public async Task<IdentityResult> AddAsync(User user, string password, string roleName)
    {
        // Add the user
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Set the user role
            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded) return roleResult;
        }

        return result;
    }

    public virtual async Task<IdentityResult> UpdateAsync(User newUserData, string? newPassword, string? newRoleName)
    {
        var user = await FindByIdAsync(newUserData.Id) ??
            throw new InvalidOperationException("User not found");

        // Assign the attributes
        user.UserName = newUserData.UserName;
        user.Email = newUserData.Email;
        user.CustomerId = newUserData.CustomerId;

        // Update the password if required
        if (!string.IsNullOrEmpty(newPassword))
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, newPassword);
            if (!result.Succeeded) return result;
        }

        // Update the user's role if needed
        if (!string.IsNullOrEmpty(newRoleName))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!newRoleName.Equals(currentRoles.FirstOrDefault()))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var roleResult = await _userManager.AddToRoleAsync(user, newRoleName);
                if (!roleResult.Succeeded) return roleResult;
            }
        }

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(User user) =>
        await _userManager.DeleteAsync(user);

    public async Task<SignInResult> LoginAsync(string username, string password) =>
        await _signInManager.PasswordSignInAsync(
            username,
            password,
            true,
            lockoutOnFailure: false
        );

    public async Task LogoutAsync() =>
        await _signInManager.SignOutAsync();

    public async Task<bool> IsPasswordValid(User user, string password) =>
        await _userManager.CheckPasswordAsync(user, password);

    public async Task<IList<IdentityRole<int>>> FetchAllRolesAsync() =>
        await _dbContext.Roles.ToListAsync();

    public async Task<IdentityRole<int>> GetRoleAsync(User user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        var role = await _roleManager.FindByNameAsync(roleNames.First());

        return role!;
    }

    public async Task<string> GenerateEmailConfirmationCodeAsync(User user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
    }
}
