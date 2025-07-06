using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static EndPointCommerce.Domain.Interfaces.IOrderRepository;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Order entity.
/// </summary>
public class OrderRepository : BaseAuditRepository<Order>, IOrderRepository
{
    public OrderRepository(EndPointCommerceDbContext context, IHttpContextAccessor httpContextAccessorAccessor) : base(context, httpContextAccessorAccessor)
    {
    }

    public async Task<IList<Order>> FetchAllByCustomerIdAsync(int customerId)
    {
        var query = DbSet()
            .IncludeEverything()
            .Where(p => !p.Deleted && p.CustomerId == customerId);
        return await query.OrderBy(p => p.Id).ToListAsync();
    }

    public async Task<Order?> FindByIdWithItemsAsync(int id)
    {
        return await DbSet()
            .IncludeEverything()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Order?> FindByGuidWithItemsAsync(Guid guid)
    {
        return await DbSet()
            .IncludeEverything()
            .Where(x => x.OrderGuid == guid)
            .FirstOrDefaultAsync();
    }

    protected List<CountPerGroup> AddMissingDaysToGenericCountListResult(List<CountPerGroup> list)
    {
        var index = 0;
        var startDate = DateTime.UtcNow.Date.AddDays(-7);
        for (var day = startDate; day.Date <= DateTime.UtcNow.Date; day = day.AddDays(1))
        {
            if (!list.Where(x => x.Group.Equals(day.ToString("yyyy-MM-dd"))).Any())
            {
                list.Insert(index, new CountPerGroup()
                {
                    Group = day.ToString("yyyy-MM-dd"),
                    Value = 0.0M
                });
            }
            index++;
        }
        return list;
    }

    /// <summary>
    /// Retrieve the sorted, active list of order counts from the last seven days
    /// </summary>
    public async Task<List<CountPerGroup>> FetchOrderCountsFromLastSevenDaysAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-7);

        var listQuery = DbSet()
            .Include(x => x.Coupon)
            .Where(x => x.Deleted != true && x.DateCreated >= startDate);

        var list = await listQuery
            .GroupBy(p => p.DateCreated!.Value.Date)
            .Select(group => new
            {
                key = group.Key,
                value = group.Count()
            })
            .OrderBy(x => x.key)
            .Select(x => new CountPerGroup
            {
                Group = x.key.ToString("yyyy-MM-dd"),
                Value = x.value
            })
            .ToListAsync();

        list = AddMissingDaysToGenericCountListResult(list);

        return list;
    }

    /// <summary>
    /// Retrieve the sorted, active list of order amounts from the last seven days
    /// </summary>
    public async Task<List<CountPerGroup>> FetchOrderAmountsFromLastSevenDaysAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-7);
        var listQuery = DbSet()
            .Include(x => x.Coupon)
            .Where(x => x.Deleted != true && x.DateCreated >= startDate);

        var list = await listQuery
            .GroupBy(p => p.DateCreated!.Value.Date)
            .Select(group => new
            {
                key = group.Key,
                value = group.Sum(x => x.Total)
            })
            .OrderBy(x => x.key)
            .Select(x => new CountPerGroup
            {
                Group = x.key.ToString("yyyy-MM-dd"),
                Value = x.value
            })
            .ToListAsync();

        list = AddMissingDaysToGenericCountListResult(list);

        return list;
    }

    /// <summary>
    /// Retrieve the amount of today's sales
    /// </summary>
    public async Task<decimal> GetTodaysSales()
    {
        var listQuery = DbSet()
            .Include(x => x.Coupon)
            .Where(x => x.Deleted != true && x.DateCreated!.Value.Date == DateTime.UtcNow.Date);

        return await listQuery.SumAsync(x => x.Total);
    }

    /// <summary>
    /// Retrieve the amount of this month's sales
    /// </summary>
    public async Task<decimal> GetMonthSales()
    {
        var listQuery = DbSet()
            .Include(x => x.Coupon)
            .Where(x =>
                x.Deleted != true &&
                x.DateCreated!.Value.Month == DateTime.UtcNow.Date.Month &&
                x.DateCreated!.Value.Year == DateTime.UtcNow.Date.Year
            );

        return await listQuery.SumAsync(x => x.Total);
    }
}

internal static class OrderQueryExtensions
{
    public static IQueryable<Order> IncludeEverything(this IQueryable<Order> query) =>
        query
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
            .Include(x => x.ShippingAddress).ThenInclude(x => x.State)
            .Include(x => x.BillingAddress).ThenInclude(x => x.State)
            .Include(x => x.PaymentMethod)
            .Include(x => x.Status);
}