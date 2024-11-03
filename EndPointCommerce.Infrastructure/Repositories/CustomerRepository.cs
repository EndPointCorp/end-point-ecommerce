using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Customer entity.
/// </summary>
public class CustomerRepository : BaseAuditRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(EndPointCommerceDbContext context, IHttpContextAccessor httpContextAccessorAccessor) : base(context, httpContextAccessorAccessor)
    {
    }

    /// <summary>
    /// Retrieve the sorted, active list of customers
    /// </summary>
    public async Task<IEnumerable<Customer>> FetchAllAsync()
    {
        return await DbSet().Where(x => x.Deleted != true).OrderBy(x => x.Name).ThenBy(x => x.LastName).ToListAsync();
    }

    public async Task<Customer?> FindByEmailAsync(string email) =>
        await DbSet().FirstOrDefaultAsync(c => c.Email == email);

    /// <summary>
    /// Retrieve the count of customers from the current month
    /// </summary>
    public async Task<int> GetMonthCustomersCount()
    {
        return await DbSet().Where(x => x.Deleted != true && x.DateCreated!.Value.Month == DateTime.Today.Month &&
            x.DateCreated!.Value.Year == DateTime.Today.Year).CountAsync();
    }

}