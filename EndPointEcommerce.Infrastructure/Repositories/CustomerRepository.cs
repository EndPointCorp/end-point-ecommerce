// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Customer entity.
/// </summary>
public class CustomerRepository : BaseAuditRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(EndPointEcommerceDbContext context, IHttpContextAccessor httpContextAccessorAccessor) : base(context, httpContextAccessorAccessor)
    {
    }

    /// <summary>
    /// Retrieve the sorted, active list of customers
    /// </summary>
    public async Task<IEnumerable<Customer>> FetchAllAsync()
    {
        return await DbSet().Where(x => x.Deleted != true).OrderBy(x => x.Name).ThenBy(x => x.LastName).ToListAsync();
    }

    public async Task<Customer?> FindByEmailAsync(string email) =>
        await DbSet().FirstOrDefaultAsync(c => c.Email == email);

    /// <summary>
    /// Retrieve the count of customers from the current month
    /// </summary>
    public async Task<int> GetMonthCustomersCount()
    {
        return await DbSet().Where(x => x.Deleted != true && x.DateCreated!.Value.Month == DateTime.UtcNow.Date.Month &&
            x.DateCreated!.Value.Year == DateTime.UtcNow.Date.Year).CountAsync();
    }

}