using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Country entity.
/// </summary>
public class CountryRepository : BaseRepository<Country>, ICountryRepository
{
    public CountryRepository(EndPointCommerceDbContext context) : base(context) { }

    /// <summary>
    /// Retrieve the sorted list of countries
    /// </summary>
    public async Task<IList<Country>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<IList<Country>> FetchAllEnabledAsync()
    {
        return await DbSet().Where(x => x.IsEnabled).OrderBy(x => x.Name).ToListAsync();
    }
}