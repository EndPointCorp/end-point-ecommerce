using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Infrastructure.Data;

namespace EndPointEcommerce.AdminPortal.Services;

public class CustomerSearchResultItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? DateCreated { get; set; }
    public string? EditUrl { get; set; }
}

public interface ICustomerSearcher
{
    Task<SearchResult<CustomerSearchResultItem>> Search(SearchParameters parameters, IUrlBuilder url);
}

public class CustomerSearcher : BaseEntitySearcher<CustomerSearchResultItem, Customer>, ICustomerSearcher
{
    public CustomerSearcher(EndPointEcommerceDbContext context) : base(context) { }

    protected override IQueryable<Customer> InitQuery() =>
        _context.Customers.AsQueryable();

    protected override IQueryable<Customer> ApplyFilters(IQueryable<Customer> query, string searchValue) =>
        query.Where(
            c => c.Name.ToLower().Contains(searchValue) ||
            (
                c.LastName != null &&
                c.LastName.ToLower().Contains(searchValue)
            ) ||
            c.Email.ToLower().Contains(searchValue)
        );

    protected override Dictionary<(string, string), Func<IQueryable<Customer>, IQueryable<Customer>>>
        OrderByStatements =>
            new()
            {
                [("name", "asc")] = q => q.OrderBy(c => c.Name),
                [("lastName", "asc")] = q => q.OrderBy(c => c.LastName),
                [("email", "asc")] = q => q.OrderBy(c => c.Email),
                [("dateCreated", "asc")] = q => q.OrderBy(c => c.DateCreated),

                [("name", "desc")] = q => q.OrderByDescending(c => c.Name),
                [("lastName", "desc")] = q => q.OrderByDescending(c => c.LastName),
                [("email", "desc")] = q => q.OrderByDescending(c => c.Email),
                [("dateCreated", "desc")] = q => q.OrderByDescending(c => c.DateCreated),
            };

    protected override IQueryable<CustomerSearchResultItem> ApplySelect(
        IQueryable<Customer> query,
        IUrlBuilder url
    ) =>
        query.Select(
            entity => new CustomerSearchResultItem()
            {
                Id = entity.Id,
                Name = entity.Name,
                LastName = entity.LastName,
                Email = entity.Email,
                DateCreated = entity.DateCreated!.Value.ToString("G"),
                EditUrl = url.Build("./Edit", new { entity.Id })
            }
        );
}
