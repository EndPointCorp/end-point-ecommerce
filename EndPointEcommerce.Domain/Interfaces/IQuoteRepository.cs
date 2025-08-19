// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Quote repository interface.
/// </summary>
public interface IQuoteRepository : IBaseRepository<Quote>
{
    Task<Quote?> FindOpenByIdAsync(int id);
    Task<Quote?> FindOpenByCustomerIdAsync(int customerId);
    Task<Quote> CreateNewAsync(int? customerId);
}
