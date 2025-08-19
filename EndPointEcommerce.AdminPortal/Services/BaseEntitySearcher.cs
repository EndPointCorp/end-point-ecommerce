// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.AdminPortal.Services;

public class SearchParameters
{
    public required int Draw { get; set; }
    public required int Start { get; set; }
    public required int Length { get; set; }

    public IDictionary<string, string>? Search { get; set; }
    public IDictionary<string, string>[]? Order { get; set; }
}

public class SearchResult<TResultItem>
{
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public List<TResultItem> Data { get; set; } = [];
}

public class SearchError<TResultItem> : SearchResult<TResultItem>
{
    public SearchError(string error)
    {
        Error = error;
    }

    public string? Error { get; set; }
}

public abstract class BaseEntitySearcher<TResultItem, TEntity>
{
    protected readonly EndPointEcommerceDbContext _context;

    public BaseEntitySearcher(EndPointEcommerceDbContext context)
    {
        _context = context;
    }

    public async Task<SearchResult<TResultItem>> Search(SearchParameters parameters, IUrlBuilder url)
    {
        var query = InitQuery();

        var searchValue = parameters.Search?["value"]?.ToLower();
        if (!string.IsNullOrEmpty(searchValue)) query = ApplyFilters(query, searchValue);

        query = ApplySorting(query, parameters);

        try
        {
            return new SearchResult<TResultItem>()
            {
                Draw = parameters.Draw,
                RecordsTotal = InitQuery().Count(),
                RecordsFiltered = query.Count(),
                Data = await RunQuery(query, parameters, url),
            };
        }
        catch (Exception ex)
        {
            return new SearchError<TResultItem>(ex.Message);
        }
    }

    protected abstract IQueryable<TEntity> InitQuery();
    protected abstract IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, string searchValue);
    protected abstract IQueryable<TResultItem> ApplySelect(IQueryable<TEntity> query, IUrlBuilder url);

    protected abstract Dictionary<(string, string), Func<IQueryable<TEntity>, IQueryable<TEntity>>>
        OrderByStatements { get; }

    private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, SearchParameters parameters)
    {
        var orderByField = parameters.Order?.FirstOrDefault()?["name"];
        var orderByDirection = parameters.Order?.FirstOrDefault()?["dir"];

        if (!string.IsNullOrEmpty(orderByField) && !string.IsNullOrEmpty(orderByDirection))
        {
            var key = (orderByField, orderByDirection);

            if (OrderByStatements.ContainsKey(key))
            {
                query = OrderByStatements[key].Invoke(query);
            }
        }

        return query;
    }

    private async Task<List<TResultItem>> RunQuery(
        IQueryable<TEntity> query,
        SearchParameters parameters,
        IUrlBuilder url
    ) {
        query = query.Skip(parameters.Start).Take(parameters.Length);
        return await ApplySelect(query, url).ToListAsync();
    }
}
