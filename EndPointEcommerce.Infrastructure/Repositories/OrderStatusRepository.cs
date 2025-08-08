using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Order status entity.
/// </summary>
public class OrderStatusRepository : BaseRepository<OrderStatus>, IOrderStatusRepository
{
    public OrderStatusRepository(EndPointEcommerceDbContext context) : base(context) { }

    /// <summary>
    /// Retrieve the sorted, active list of order status
    /// </summary>
    public async Task<IEnumerable<OrderStatus>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<OrderStatus?> FindByNameAsync(string name) =>
        await DbSet().FirstOrDefaultAsync(c => c.Name == name);

    public async Task<OrderStatus> GetPending() =>
        await FindByNameAsync(OrderStatus.PENDING) ??
            throw new InvalidOperationException("The database is not properly configured.");
}