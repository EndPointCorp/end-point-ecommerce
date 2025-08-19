// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// ProductImage repository interface.
/// </summary>
public interface IProductImageRepository : IBaseRepository<ProductImage>
{
    public Task<IEnumerable<ProductImage>> FetchAllAsync();
}
