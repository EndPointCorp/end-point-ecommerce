using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a State entity.
/// </summary>
public class StateRepository : BaseRepository<State>, IStateRepository
{
    public StateRepository(EndPointCommerceDbContext context) : base(context) { }

    /// <summary>
    /// Retrieve the sorted, active list of states
    /// </summary>
    public async Task<IEnumerable<State>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.Name).ToListAsync();
    }

}