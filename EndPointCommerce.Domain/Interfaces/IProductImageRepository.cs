using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// ProductImage repository interface.
/// </summary>
public interface IProductImageRepository : IBaseRepository<ProductImage>
{
    public Task<IEnumerable<ProductImage>> FetchAllAsync();
}
