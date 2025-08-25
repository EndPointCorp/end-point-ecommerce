// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Order item entity.
/// </summary>
public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(EndPointEcommerceDbContext context) : base(context) { }

    public async Task<OrderItem?> FindByIdWithOrderAsync(int id)
    {
        return await DbSet().Where(x => x.Id == id).Include(x => x.Order).FirstOrDefaultAsync();
    }
}