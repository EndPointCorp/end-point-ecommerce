// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Quote item entity.
/// </summary>
public class QuoteItemRepository : BaseRepository<QuoteItem>, IQuoteItemRepository
{
    public QuoteItemRepository(EndPointEcommerceDbContext context) : base(context) { }

    public override async Task DeleteAsync(QuoteItem entity)
    {
        DbSet().Remove(entity);
        await DbContext.SaveChangesAsync();
    }
}
