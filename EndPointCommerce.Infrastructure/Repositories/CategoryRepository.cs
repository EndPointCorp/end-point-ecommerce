using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

public class CategoryRepository : BaseAuditRepository<Category>, ICategoryRepository
{
    private DbSet<Category> Categories => DbSet();

    public CategoryRepository(EndPointCommerceDbContext context, IHttpContextAccessor httpContextAccessor)
        : base(context, httpContextAccessor)
    {
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await Categories.AnyAsync(c => c.Id == id);
    }

    public override async Task<Category?> FindByIdAsync(int id, bool includeDeleted = false)
    {
        var query = Categories
            .Include(c => c.MainImage)
            .Where(p => p.Id == id);

        query = AddIncludeDeletedExpression(query, includeDeleted);

        return await query.FirstOrDefaultAsync();
    }


    public async Task<Category?> FindByNameAsync(string name)
    {
        return await Categories.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Category?> FindByUrlKeyAsync(string urlKey)
    {
        return await Categories.FirstOrDefaultAsync(c => c.UrlKey == urlKey);
    }

    public async Task<IList<Category>> FetchAllAsync(bool enabledOnly = false)
    {
        IQueryable<Category> query = Categories.Include(p => p.MainImage).OrderBy(c => c.Name);
        query = AddEnabledOnlyExpression(query, enabledOnly);

        return await query.ToListAsync();
    }

    public async Task DeleteMainImage(Category category)
    {
        if (category.MainImage is null) return;

        DbContext.CategoryImages.Remove(category.MainImage);
        category.MainImageId = null;

        await UpdateAsync(category);
    }

    private static IQueryable<Category> AddEnabledOnlyExpression(IQueryable<Category> query, bool enabledOnly) =>
        enabledOnly ? query.Where(p => p.IsEnabled) : query;
}
