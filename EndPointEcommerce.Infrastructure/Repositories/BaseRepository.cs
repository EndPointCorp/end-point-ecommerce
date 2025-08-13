using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Base class for a GenericEntity repository class.
/// </summary>
public abstract class BaseRepository<T>: IBaseRepository<T> where T : BaseEntity
{
    protected EndPointEcommerceDbContext DbContext { get; set; }

    protected BaseRepository(EndPointEcommerceDbContext context)
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