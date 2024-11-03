using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Quote item entity.
/// </summary>
public class QuoteItemRepository : BaseRepository<QuoteItem>, IQuoteItemRepository
{
    public QuoteItemRepository(EndPointCommerceDbContext context) : base(context) { }

    public override async Task DeleteAsync(QuoteItem entity)
    {
        DbSet().Remove(entity);
        await DbContext.SaveChangesAsync();
    }
}
