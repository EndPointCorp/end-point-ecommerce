// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Order status repository interface.
/// </summary>
public interface IOrderStatusRepository : IBaseRepository<OrderStatus>
{
    Task<IEnumerable<OrderStatus>> FetchAllAsync();
    Task<OrderStatus> GetPending();
}
