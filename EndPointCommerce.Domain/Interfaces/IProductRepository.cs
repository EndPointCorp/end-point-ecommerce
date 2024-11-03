using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Product repository interface.
/// </summary>
public interface IProductRepository : IBaseRepository<Product>
{
    public class SearchSuggestion
    {
        public string Term { get; set; } = "";
        public int Count { get; set; }
    }

    Task<IList<Product>> FetchAllBySearchQueryAsync(string query);
    Task<IList<SearchSuggestion>> FetchAllSuggestionsBySearchQueryAsync(string query);

    Task<IList<Product>> FetchAllAsync(bool enabledOnly = false);
    Task<bool> ExistsAsync(int id);
    Task<Product?> FindByIdAsync(int id, bool enabledOnly = false, bool includeDeleted = false);
    Task<Product?> FindByNameAsync(string name);
    Task<Product?> FindBySkuAsync(string sku);
    Task<Product?> FindByUrlKeyAsync(string urlKey, bool enabledOnly = false);
    Task<IList<Product>> FetchAllByCategoryIdAsync(int categoryId, bool enabledOnly = false);
    Task<IList<Product>> FetchAllByCategoryUrlKeyAsync(string categoryUrlKey, bool enabledOnly = false);
    Task DeleteMainImage(Product product);
    Task DeleteThumbnailImage(Product product);
    Task DeleteAdditionalImage(ProductImage imageToDelete);
    Task<IList<Product>> FetchAllByNameAsync(string name, bool enabledOnly = false);
    Task<int> GetActiveProductCount();
}
