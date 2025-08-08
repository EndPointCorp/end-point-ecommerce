using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static EndPointEcommerce.Domain.Interfaces.IProductRepository;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Product entity.
/// </summary>
public class ProductRepository : BaseAuditRepository<Product>, IProductRepository
{
    private DbSet<Product> Products => DbSet();

    public ProductRepository(EndPointEcommerceDbContext context, IHttpContextAccessor httpContextAccessor)
        : base(context, httpContextAccessor)
    {
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await Products
            .Include(p => p.Category)
            .AnyAsync(c => c.Id == id);
    }

    public async Task<Product?> FindByIdAsync(int id, bool enabledOnly = false, bool includeDeleted = false)
    {
        var query = Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(enabledOnly)
            .Where(p => p.Id == id);

        query = AddIncludeDeletedExpression(query, includeDeleted);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Product?> FindBySkuAsync(string sku)
    {
        return await Products.FirstOrDefaultAsync(c => c.Sku == sku);
    }

    public async Task<Product?> FindByNameAsync(string name)
    {
        return await Products.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Product?> FindByUrlKeyAsync(string urlKey, bool enabledOnly = false)
    {
        return await Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(enabledOnly)
            .FirstOrDefaultAsync(c => c.UrlKey == urlKey);
    }

    /// <summary>
    /// Retrieve the sorted, active list of products
    /// </summary>
    public async Task<IList<Product>> FetchAllAsync(bool enabledOnly = false)
    {
        var query = Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(enabledOnly);

        query = AddIncludeDeletedExpression(query, false);

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<IList<Product>> FetchAllByCategoryIdAsync(int categoryId, bool enabledOnly = false)
    {
        var query = Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(enabledOnly)
            .Where(p => p.CategoryId == categoryId);

        query = AddIncludeDeletedExpression(query, false);

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<IList<Product>> FetchAllByCategoryUrlKeyAsync(string categoryUrlKey, bool enabledOnly = false)
    {
        var query = Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(enabledOnly)
            .Where(p => p.Category!.UrlKey == categoryUrlKey);

        query = AddIncludeDeletedExpression(query, false);

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<IList<Product>> FetchAllByNameAsync(string name, bool enabledOnly = false)
    {
        var query = Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(enabledOnly)
            .Where(x => x.Name.Trim().ToLower().Equals(name.Trim().ToLower()));

        query = AddIncludeDeletedExpression(query, false);

        return await query.ToListAsync();
    }

    public async Task<IList<Product>> FetchAllBySearchQueryAsync(string searchQuery)
    {
        var query = Products
            .IncludeCategory()
            .IncludeImages()
            .MaybeEnabledOnly(true);

        if (searchQuery.EndsWith(")") && searchQuery.Contains(" ("))
        {
            var trimmedSearchResult = searchQuery.Split('(')[0].Trim();
            query = query.Where(x => x.Name.Equals(trimmedSearchResult));
        }
        else
        {
            query = query.Where(x => x.SearchVector!.Matches(EF.Functions.ToTsQuery(searchQuery + ":*")));
        }

        query = AddIncludeDeletedExpression(query, false);

        return await query.ToListAsync();
    }

    public async Task<IList<SearchSuggestion>> FetchAllSuggestionsBySearchQueryAsync(string searchQuery)
    {
        var query = Products
            .MaybeEnabledOnly(true)
            .Where(x => x.SearchVector!.Matches(EF.Functions.ToTsQuery(searchQuery + ":*")));

        query = AddIncludeDeletedExpression(query, false);

        // Group the list by search term results
        var list = await query
            .OrderByDescending(td => td.SearchVector!.Rank(EF.Functions.ToTsQuery(searchQuery + ":*")))
            .GroupBy(p => new { p.Name, p.Sku })
            .Select(group => new SearchSuggestion
            {
                Term = $"{group.Key.Name} ({group.Key.Sku})",
                Count = group.Count()
            })
            .OrderBy(x => x.Count)
            .Take(10)
            .ToListAsync();

        // Add the total count for the search term
        list.Insert(0, new SearchSuggestion()
        {
            Term = searchQuery,
            Count = await query.CountAsync()
        });

        return list;
    }

    public async Task DeleteMainImage(Product product)
    {
        await DeleteImage(
            product.MainImage,
            product,
            p => p.MainImageId = null
        );
    }

    public async Task DeleteThumbnailImage(Product product)
    {
        await DeleteImage(
            product.ThumbnailImage,
            product,
            p => p.ThumbnailImageId = null
        );
    }

    public async Task DeleteAdditionalImage(ProductImage imageToDelete)
    {
        DbContext.ProductImages.Remove(imageToDelete);
        await DbContext.SaveChangesAsync();
    }

    private async Task DeleteImage(ProductImage? imageToDelete, Product productToUpdate, Action<Product> dissociate)
    {
        if (imageToDelete is null) return;

        dissociate(productToUpdate);
        await UpdateAsync(productToUpdate);

        DbContext.ProductImages.Remove(imageToDelete);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieve the active product count
    /// </summary>
    public async Task<int> GetActiveProductCount()
    {
        return await Products.MaybeEnabledOnly(true).CountAsync();
    }
}

internal static class ProductQueryExtensions
{
    public static IQueryable<Product> IncludeCategory(this IQueryable<Product> query) =>
        query.Include(p => p.Category);

    public static IQueryable<Product> IncludeImages(this IQueryable<Product> query) =>
        query
            .Include(p => p.MainImage)
            .Include(p => p.ThumbnailImage)
            .Include(p => p.AdditionalImages);

    public static IQueryable<Product> MaybeEnabledOnly(this IQueryable<Product> query, bool enabledOnly)
    {
        if (enabledOnly)
        {
            query = query.Where(p =>
                p.IsEnabled && // the product is enabled and...
                (p.Category == null || p.Category.IsEnabled) // its category is also enabled.
            );
        }

        return query;
    }
}