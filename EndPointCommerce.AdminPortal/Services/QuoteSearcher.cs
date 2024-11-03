using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.AdminPortal.Services;

public class QuoteSearchResultItem
{
    public int Id { get; set; }
    public string? DateCreated { get; set; }
    public string? Email { get; set; }
    public bool? IsOpen { get; set; }
    public string? ShippingAddressStateName { get; set; }
    public string? Total { get; set; }
    public string? DetailsUrl { get; set; }
}

public interface IQuoteSearcher
{
    Task<SearchResult<QuoteSearchResultItem>> Search(SearchParameters parameters, IUrlBuilder url);
}

public class QuoteSearcher : BaseEntitySearcher<QuoteSearchResultItem, Quote>, IQuoteSearcher
{
    public QuoteSearcher(EndPointCommerceDbContext context) : base(context) { }

    protected override IQueryable<Quote> InitQuery() =>
        _context.Quotes
            .Include(x => x.Customer)
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .Include(x => x.ShippingAddress).ThenInclude(x => x!.State)
            .OrderByDescending(x => x.Id)
            .AsQueryable();

    protected override IQueryable<Quote> ApplyFilters(IQueryable<Quote> query, string searchValue) =>
        query.Where(q =>
            q.Id.ToString().ToLower().Contains(searchValue) ||
            (
                q.DateCreated != null &&
                q.DateCreated.Value.ToString().ToLower().Contains(searchValue)
            ) ||
            (
                q.Customer != null &&
                q.Customer.Email.ToLower().Contains(searchValue)
            ) ||
            (
                q.Email != null &&
                q.Email.ToLower().Contains(searchValue)
            ) ||
            q.ShippingAddress!.State.Name.ToLower().Contains(searchValue)
        );

    protected override Dictionary<(string, string), Func<IQueryable<Quote>, IQueryable<Quote>>>
        OrderByStatements =>
            new()
            {
                [("id", "asc")] = q => q.OrderBy(q => q.Id),
                [("dateCreated", "asc")] = q => q.OrderBy(q => q.DateCreated),
                [("email", "asc")] = q => q.OrderBy(q => q.Customer != null ? q.Customer.Email : q.Email),
                [("isOpen", "asc")] = q => q.OrderBy(q => q.IsOpen),
                [("shippingAddressStateName", "asc")] = q => q.OrderBy(q => q.ShippingAddress!.State.Name),

                [("id", "desc")] = q => q.OrderByDescending(q => q.Id),
                [("dateCreated", "desc")] = q => q.OrderByDescending(q => q.DateCreated),
                [("email", "desc")] = q => q.OrderByDescending(q => q.Customer != null ? q.Customer.Email : q.Email),
                [("isOpen", "desc")] = q => q.OrderByDescending(q => q.IsOpen),
                [("shippingAddressStateName", "desc")] = q => q.OrderByDescending(q => q.ShippingAddress!.State.Name),
            };

    protected override IQueryable<QuoteSearchResultItem> ApplySelect(
        IQueryable<Quote> query,
        IUrlBuilder url
    ) =>
        query.Select(
            entity => new QuoteSearchResultItem()
            {
                Id = entity.Id,
                DateCreated = entity.DateCreated!.Value.ToString("G"),
                Email = entity.Customer != null ? entity.Customer.Email : entity.Email,
                IsOpen = entity.IsOpen,
                ShippingAddressStateName = entity.ShippingAddress != null ? entity.ShippingAddress.State.Name : null,
                Total = string.Format("{0:C}", entity.Total),
                DetailsUrl = url.Build("./Details", new { entity.Id })
            }
        );
}