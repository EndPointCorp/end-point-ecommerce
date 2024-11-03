using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;

namespace EndPointCommerce.AdminPortal.Services;

public class CouponSearchResultItem
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public decimal? Discount { get; set; }
    public bool? IsDiscountFixed { get; set; }
    public string? DateCreated { get; set; }
    public string? DateModified { get; set; }
    public string? EditUrl { get; set; }
}

public interface ICouponSearcher
{
    Task<SearchResult<CouponSearchResultItem>> Search(SearchParameters parameters, IUrlBuilder url);
}

public class CouponSearcher : BaseEntitySearcher<CouponSearchResultItem, Coupon>, ICouponSearcher
{
    public CouponSearcher(EndPointCommerceDbContext context) : base(context) { }

    protected override IQueryable<Coupon> InitQuery() =>
        _context.Coupons.AsQueryable();

    protected override IQueryable<Coupon> ApplyFilters(IQueryable<Coupon> query, string searchValue) =>
        query.Where(c =>
            c.Code.ToLower().Contains(searchValue)
        );

    protected override Dictionary<(string, string), Func<IQueryable<Coupon>, IQueryable<Coupon>>>
        OrderByStatements =>
            new()
            {
                [("code", "asc")] = q => q.OrderBy(c => c.Code),
                [("discount", "asc")] = q => q.OrderBy(c => c.Discount),
                [("isDiscountFixed", "asc")] = q => q.OrderBy(c => c.IsDiscountFixed),
                [("dateCreated", "asc")] = q => q.OrderBy(c => c.DateCreated),
                [("dateModified", "asc")] = q => q.OrderBy(c => c.DateModified),

                [("code", "desc")] = q => q.OrderByDescending(c => c.Code),
                [("discount", "desc")] = q => q.OrderByDescending(c => c.Discount),
                [("isDiscountFixed", "desc")] = q => q.OrderByDescending(c => c.IsDiscountFixed),
                [("dateCreated", "desc")] = q => q.OrderByDescending(c => c.DateCreated),
                [("dateModified", "desc")] = q => q.OrderByDescending(c => c.DateModified),
            };

    protected override IQueryable<CouponSearchResultItem> ApplySelect(
        IQueryable<Coupon> query,
        IUrlBuilder url
    ) =>
        query.Select(
            entity => new CouponSearchResultItem()
            {
                Id = entity.Id,
                Code = entity.Code,
                Discount = entity.Discount,
                IsDiscountFixed = entity.IsDiscountFixed,
                DateCreated = entity.DateCreated!.Value.ToString("G"),
                DateModified = entity.DateModified!.Value.ToString("G"),
                EditUrl = url.Build("./Edit", new { entity.Id })
            }
        );
}
