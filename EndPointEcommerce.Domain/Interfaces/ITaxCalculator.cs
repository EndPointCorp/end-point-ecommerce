// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Tax service interface.
/// </summary>
public interface ITaxCalculator
{
    public class TaxResponse
    {
        public decimal AmountToCollect { get; set; }
    }

    Task<TaxResponse?> Calculate(Quote quote);
}
