// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Country repository interface.
/// </summary>
public interface ICountryRepository : IBaseRepository<Country>
{
    public Task<IList<Country>> FetchAllAsync();
    public Task<IList<Country>> FetchAllEnabledAsync();
}
