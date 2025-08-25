// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Country entity.
/// </summary>
public class CountryRepository : BaseRepository<Country>, ICountryRepository
{
    public CountryRepository(EndPointEcommerceDbContext context) : base(context) { }

    /// <summary>
    /// Retrieve the sorted list of countries
    /// </summary>
    public async Task<IList<Country>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<IList<Country>> FetchAllEnabledAsync()
    {
        return await DbSet().Where(x => x.IsEnabled).OrderBy(x => x.Name).ToListAsync();
    }
}