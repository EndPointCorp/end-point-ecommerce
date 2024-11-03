using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Order status repository interface.
/// </summary>
public interface IOrderStatusRepository : IBaseRepository<OrderStatus>
{
    Task<IEnumerable<OrderStatus>> FetchAllAsync();
    Task<OrderStatus> GetPending();
}
