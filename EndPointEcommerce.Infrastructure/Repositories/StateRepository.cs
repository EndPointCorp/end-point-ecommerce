// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a State entity.
/// </summary>
public class StateRepository : BaseRepository<State>, IStateRepository
{
    public StateRepository(EndPointEcommerceDbContext context) : base(context) { }

    /// <summary>
    /// Retrieve the sorted, active list of states
    /// </summary>
    public async Task<IEnumerable<State>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.Name).ToListAsync();
    }

}