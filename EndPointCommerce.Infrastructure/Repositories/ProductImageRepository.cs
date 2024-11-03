using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a ProductImage entity.
/// </summary>
public class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(EndPointCommerceDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieve the sorted, active list of product imaes
    /// </summary>
    public async Task<IEnumerable<ProductImage>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.FileName).ToListAsync();
    }

}