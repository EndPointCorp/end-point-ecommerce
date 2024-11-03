using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.AdminPortal.Services;

public class UserSearchResultItem
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? EditUrl { get; set; }
}

public interface IUserSearcher
{
    Task<SearchResult<UserSearchResultItem>> Search(SearchParameters parameters, IUrlBuilder url);
}

public class UserWithRole
{
    public User User { get; set; } = default!;
    public IdentityRole<int> Role { get; set; } = default!;
}

public class UserSearcher : BaseEntitySearcher<UserSearchResultItem, UserWithRole>, IUserSearcher
{
    public UserSearcher(EndPointCommerceDbContext context) : base(context) { }

    protected override IQueryable<UserWithRole> InitQuery() =>
        from u in _context.Users
        join ur in _context.UserRoles on u.Id equals ur.UserId
        join r in _context.Roles on ur.RoleId equals r.Id
        select new UserWithRole { User = u, Role = r };

    protected override IQueryable<UserWithRole> ApplyFilters(IQueryable<UserWithRole> query, string searchValue) =>
        query.Where(u =>
            (
                u.User.Email != null &&
                u.User.Email.ToLower().Contains(searchValue)
            ) ||
            u.Role.Name!.ToLower().Contains(searchValue)
        );

    protected override Dictionary<(string, string), Func<IQueryable<UserWithRole>, IQueryable<UserWithRole>>>
        OrderByStatements =>
            new()
            {
                [("email", "asc")] = q => q.OrderBy(u => u.User.Email),
                [("role", "asc")] = q => q.OrderBy(u => u.Role.Name),

                [("email", "desc")] = q => q.OrderByDescending(u => u.User.Email),
                [("role", "desc")] = q => q.OrderByDescending(u => u.Role.Name),
            };

    protected override IQueryable<UserSearchResultItem> ApplySelect(
        IQueryable<UserWithRole> query,
        IUrlBuilder url
    ) =>
        query.Select(
            entity => new UserSearchResultItem()
            {
                Id = entity.User.Id,
                Email = entity.User.Email,
                Role = entity.Role.Name,
                EditUrl = url.Build("./Edit", new { entity.User.Id })
            }
        );
}
