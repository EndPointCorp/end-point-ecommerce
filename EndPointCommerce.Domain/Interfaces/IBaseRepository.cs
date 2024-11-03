using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Base repository interface.
/// </summary>
public interface IBaseRepository<T> where T : BaseEntity
{
    public Task<T?> FindByIdAsync(int id);

    public Task<bool> Exists(int id);

    /// <summary>
    /// Add a new entity.
    /// </summary>
    public Task AddAsync(T entity);

    /// <summary>
    /// Permanently deletes an entity.
    /// </summary>
    public Task DeleteAsync(T entity);

    /// <summary>
    /// Add a new entity or update an existing entity.
    /// </summary>
    public Task SaveAsync(T entity);

    /// <summary>
    /// Update an existing entity.
    /// </summary>
    public Task UpdateAsync(T entity);

}
