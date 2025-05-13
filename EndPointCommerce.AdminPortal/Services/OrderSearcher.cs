using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.AdminPortal.Services;

public class OrderSearchResultItem
{
    public int Id { get; set; }
    public string? DateCreated { get; set; }
    public string? CustomerFullName { get; set; }
    public string? StatusName { get; set; }
    public string? BillingAddressStateName { get; set; }
    public string? Total { get; set; }
    public string? EditUrl { get; set; }
}

public interface IOrderSearcher
{
    Task<SearchResult<OrderSearchResultItem>> Search(SearchParameters parameters, IUrlBuilder url);
}

public class OrderSearcher : BaseEntitySearcher<OrderSearchResultItem, Order>, IOrderSearcher
{
    public OrderSearcher(EndPointCommerceDbContext context) : base(context) { }

    protected override IQueryable<Order> InitQuery() =>
        _context.Orders
            .Include(x => x.Status)
            .Include(x => x.Customer)
            .Include(x => x.BillingAddress).ThenInclude(x => x.State)
            .OrderByDescending(x => x.Id)
            .AsQueryable();

    protected override IQueryable<Order> ApplyFilters(IQueryable<Order> query, string searchValue) =>
        query.Where(o =>
            o.Id.ToString().ToLower().Contains(searchValue) ||
            (
                o.DateCreated != null &&
                o.DateCreated.Value.ToString().ToLower().Contains(searchValue)
            ) ||
            o.Customer.Name.ToLower().Contains(searchValue) ||
            (
                o.Customer.LastName != null &&
                o.Customer.LastName.ToLower().Contains(searchValue)
            ) ||
            (
                o.BillingAddress!.State != null &&
                o.BillingAddress.State.Name.ToLower().Contains(searchValue)
            ) ||
            o.Status.Name.ToLower().Contains(searchValue) ||
            o.Total.ToString().Contains(searchValue)
        );

    protected override Dictionary<(string, string), Func<IQueryable<Order>, IQueryable<Order>>>
        OrderByStatements =>
            new()
            {
                [("id", "asc")] = q => q.OrderBy(o => o.Id),
                [("dateCreated", "asc")] = q => q.OrderBy(o => o.DateCreated),
                [("customerFullName", "asc")] = q => q.OrderBy(o => o.Customer.Name),
                [("statusName", "asc")] = q => q.OrderBy(o => o.Status.Name),
                [("billingAddressStateName", "asc")] = q => q.OrderBy(o => o.BillingAddress.State!.Name),
                [("total", "asc")] = q => q.OrderBy(o => o.Total),

                [("id", "desc")] = q => q.OrderByDescending(o => o.Id),
                [("dateCreated", "desc")] = q => q.OrderByDescending(o => o.DateCreated),
                [("customerFullName", "desc")] = q => q.OrderByDescending(o => o.Customer.Name),
                [("statusName", "desc")] = q => q.OrderByDescending(o => o.Status.Name),
                [("billingAddressStateName", "desc")] = q => q.OrderByDescending(o => o.BillingAddress.State!.Name),
                [("total", "desc")] = q => q.OrderByDescending(o => o.Total),
            };

    protected override IQueryable<OrderSearchResultItem> ApplySelect(
        IQueryable<Order> query,
        IUrlBuilder url
    ) =>
        query.Select(
            entity => new OrderSearchResultItem()
            {
                Id = entity.Id,
                DateCreated = entity.DateCreated!.Value.ToString("G"),
                CustomerFullName = entity.Customer.FullName,
                StatusName = entity.Status.Name,
                BillingAddressStateName = entity.BillingAddress.State!.Name,
                Total = string.Format("{0:C}", entity.Total),
                EditUrl = url.Build("./Edit", new { entity.Id })
            }
        );
}