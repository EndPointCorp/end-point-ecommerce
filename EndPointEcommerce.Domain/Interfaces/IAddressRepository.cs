// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Address repository interface.
/// </summary>
public interface IAddressRepository : IBaseRepository<Address>
{
    public Task<IList<Address>> FetchAllByCustomerIdAsync(int customerId);
}
