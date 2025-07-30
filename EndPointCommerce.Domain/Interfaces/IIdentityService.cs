using EndPointCommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EndPointCommerce.Domain.Interfaces;

public interface IIdentityService
{
    Task<bool> Exists(int id);
    Task<User?> FindByIdAsync(int id);
    Task<User?> FindByUserNameAsync(string userName);
    Task<IdentityResult> AddAsync(User user, string password, string roleName);
    Task<IdentityResult> UpdateAsync(User newUserData, string? newPassword, string? newRoleName);
    Task<IdentityResult> DeleteAsync(User user);
    Task<SignInResult> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<bool> IsPasswordValid(User user, string password);
    Task<IList<IdentityRole<int>>> FetchAllRolesAsync();
    Task<IdentityRole<int>> GetRoleAsync(User user);
    Task<string> GenerateEmailConfirmationCodeAsync(User user);
}
