using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Order repository interface.
/// </summary>
public interface IOrderRepository : IBaseRepository<Order>
{
    public class CountPerGroup
    {
        public string Group { get; set; } = "";
        public decimal Value { get; set; } = 0.0M;
    }

    public Task<IList<Order>> FetchAllByCustomerIdAsync(int customerId);
    public Task<Order?> FindByIdWithItemsAsync(int id);
    public Task<Order?> FindByGuidWithItemsAsync(Guid guid);
    public Task<List<CountPerGroup>> FetchOrderCountsFromLastSevenDaysAsync();
    public Task<List<CountPerGroup>> FetchOrderAmountsFromLastSevenDaysAsync();
    public Task<decimal> GetTodaysSales();
    public Task<decimal> GetMonthSales();
}
