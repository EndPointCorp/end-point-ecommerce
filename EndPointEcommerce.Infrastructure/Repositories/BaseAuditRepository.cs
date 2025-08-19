// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Base class for an AuditEntity repository class.
/// </summary>
public abstract class BaseAuditRepository<T> : BaseRepository<T> where T : BaseAuditEntity
{
    protected IHttpContextAccessor HttpContextAccessor { get; set; }

    protected BaseAuditRepository(EndPointEcommerceDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<T?> FindByIdAsync(int id, bool includeDeleted = false)
    {
        var query = DbSet().Where(x => x.Id == id);
        query = AddIncludeDeletedExpression(query, includeDeleted);

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Add a new entity.
    /// </summary>
    public override async Task AddAsync(T entity)
    {
        entity.DateCreated = DateTime.UtcNow;
        entity.DateModified = DateTime.UtcNow;
        entity.CreatedBy = GetCurrentUser();
        entity.ModifiedBy = entity.CreatedBy;
        await base.AddAsync(entity);
    }

    /// <summary>
    /// Flags an entity as deleted.
    /// </summary>
    public override async Task DeleteAsync(T entity)
    {
        entity.Deleted = true;
        entity.DateDeleted = DateTime.UtcNow;
        entity.DeletedBy = GetCurrentUser();
        await base.UpdateAsync(entity);
    }

    /// <summary>
    /// Delete or restore an entity.
    /// </summary>
    public async Task DeleteOrRestoreAsync(T entity)
    {
        if (entity.Deleted)
            await RestoreAsync(entity);
        else
            await DeleteAsync(entity);
    }

    /// <summary>
    /// Restore an entity that was flagged as deleted.
    /// </summary>
    public async Task RestoreAsync(T entity)
    {
        entity.Deleted = false;
        entity.DateDeleted = DateTime.UtcNow;
        entity.DeletedBy = GetCurrentUser();
        await base.UpdateAsync(entity);
    }

    /// <summary>
    /// Permanently removes an entity.
    /// </summary>
    public virtual async Task RemoveAsync(T entity)
    {
        await base.DeleteAsync(entity);
    }

    /// <summary>
    /// Add a new entity or update an existing entity.
    /// </summary>
    public override async Task SaveAsync(T entity)
    {
        if (entity.Id == 0)
            await AddAsync(entity);
        else
            await UpdateAsync(entity);
    }

    /// <summary>
    /// Update an existing entity.
    /// </summary>
    public override async Task UpdateAsync(T entity)
    {
        // TODO: Should Deleted flag be updated if a deleted entity is passed in?

        // Retrieve base entity data if empty
        if (entity.CreatedBy == null)
        {
            var existingEntity = await FindByIdAsync(entity.Id, true);
            if (existingEntity != null)
            {
                entity.DateCreated = existingEntity.DateCreated;
                entity.CreatedBy = existingEntity.CreatedBy;
                entity.DateDeleted = existingEntity.DateDeleted;
                entity.DeletedBy = existingEntity.DeletedBy;

                DbContext.Entry(existingEntity).State = EntityState.Detached;
            }
        }

        entity.DateModified = DateTime.UtcNow;
        entity.ModifiedBy = GetCurrentUser();
        await base.UpdateAsync(entity);
    }

    protected static IQueryable<T> AddIncludeDeletedExpression(IQueryable<T> set, bool includeDeleted)
    {
        return includeDeleted ? set : set.Where(x => x.Deleted != true);
    }

    /// <summary>
    /// Returns the current user name based on the logged in identity.
    /// </summary>
    /// <returns>User name ID.</returns>
    private int? GetCurrentUser()
    {
        if (HttpContextAccessor.HttpContext == null) return null;
        var claim = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return null;
        return int.Parse(claim.Value);
    }
}