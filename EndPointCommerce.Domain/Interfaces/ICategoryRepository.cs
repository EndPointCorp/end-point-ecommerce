using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IList<Category>> FetchAllAsync(bool enabledOnly = false);
    Task<bool> ExistsAsync(int id);
    Task<Category?> FindByIdAsync(int id, bool includeDeleted = false);
    Task<Category?> FindByNameAsync(string name);
    Task<Category?> FindByUrlKeyAsync(string urlKey);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteMainImage(Category category);
}
