using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Order item repository interface.
/// </summary>
public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    public Task<OrderItem?> FindByIdWithOrderAsync(int id);
}
