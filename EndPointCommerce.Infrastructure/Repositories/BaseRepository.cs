using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Base class for a GenericEntity repository class.
/// </summary>
public abstract class BaseRepository<T>: IBaseRepository<T> where T : BaseEntity
{
    protected EndPointCommerceDbContext DbContext { get; set; }

    protected BaseRepository(EndPointCommerceDbContext context)
    {
        DbContext = context;
    }

    protected DbSet<T> DbSet()
    {
        return DbContext.Set<T>();
    }

    public virtual async Task<T?> FindByIdAsync(int id)
    {
        return await DbSet().FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<bool> Exists(int id)
    {
        return await DbSet().AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Add a new entity.
    /// </summary>
    public virtual async Task AddAsync(T entity)
    {
        DbSet().Add(entity);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Permanently deletes an entity.
    /// </summary>
    public virtual async Task DeleteAsync(T entity)
    {
        DbSet().Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Add a new entity or update an existing entity.
    /// </summary>
    public virtual async Task SaveAsync(T entity)
    {
        if (entity.Id == 0)
            await AddAsync(entity);
        else
            await UpdateAsync(entity);
    }

    /// <summary>
    /// Update an existing entity.
    /// </summary>
    public virtual async Task UpdateAsync(T entity)
    {
        DbSet().Update(entity);
        await DbContext.SaveChangesAsync();
    }
}