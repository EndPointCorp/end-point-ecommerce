using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Quote entity.
/// </summary>
public class QuoteRepository : BaseAuditRepository<Quote>, IQuoteRepository
{
    public QuoteRepository(
        EndPointCommerceDbContext context,
        IHttpContextAccessor httpContextAccessorAccessor
    ) : base(context, httpContextAccessorAccessor) { }

    public override async Task<Quote?> FindByIdAsync(int id)
    {
        return await DbSet()
            .IncludeEverything()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Quote?> FindOpenByIdAsync(int id)
    {
        return await DbSet()
            .IncludeEverything()
            .Where(x => x.IsOpen)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Quote?> FindOpenByCustomerIdAsync(int customerId)
    {
        return await DbSet()
            .IncludeEverything()
            .Where(x => x.IsOpen)
            .Where(x => x.CustomerId == customerId)
            .FirstOrDefaultAsync();
    }

    public async Task<Quote> CreateNewAsync(int? customerId)
    {
        var quote = new Quote
        {
            IsOpen = true,
            CustomerId = customerId
        };

        await AddAsync(quote);

        return quote;
    }
}

internal static class QuoteQueryExtensions
{
    public static IQueryable<Quote> IncludeEverything(this IQueryable<Quote> query) =>
        query
            .Include(x => x.Customer)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.ThumbnailImage)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
            .Include(x => x.ShippingAddress).ThenInclude(x => x!.Country)
            .Include(x => x.ShippingAddress).ThenInclude(x => x!.State)
            .Include(x => x.BillingAddress).ThenInclude(x => x!.Country)
            .Include(x => x.BillingAddress).ThenInclude(x => x!.State)
            .Include(x => x.Coupon);
}