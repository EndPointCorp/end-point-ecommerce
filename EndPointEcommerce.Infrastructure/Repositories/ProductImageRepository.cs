// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a ProductImage entity.
/// </summary>
public class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(EndPointEcommerceDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieve the sorted, active list of product imaes
    /// </summary>
    public async Task<IEnumerable<ProductImage>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.FileName).ToListAsync();
    }

}