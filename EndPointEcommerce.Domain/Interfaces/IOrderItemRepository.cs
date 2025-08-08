using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Order item repository interface.
/// </summary>
public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    public Task<OrderItem?> FindByIdWithOrderAsync(int id);
}
