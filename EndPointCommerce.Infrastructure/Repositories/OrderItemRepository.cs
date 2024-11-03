using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Order item entity.
/// </summary>
public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(EndPointCommerceDbContext context) : base(context) { }

    public async Task<OrderItem?> FindByIdWithOrderAsync(int id)
    {
        return await DbSet().Where(x => x.Id == id).Include(x => x.Order).FirstOrDefaultAsync();
    }
}