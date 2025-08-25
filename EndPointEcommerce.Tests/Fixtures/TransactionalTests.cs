// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Infrastructure.Data;

namespace EndPointEcommerce.Tests.Fixtures;

/// <summary>
/// Wraps all test cases within a database transaction that is rolled back when the test case finishes.
/// </summary>
public abstract class TransactionalTests : IClassFixture<DatabaseFixture>, IDisposable
{
    protected readonly EndPointEcommerceDbContext dbContext;

    public TransactionalTests(DatabaseFixture fixture)
    {
        dbContext = fixture.CreateDbContext();
        dbContext.Database.BeginTransaction();
    }

    public void Dispose()
    {
        dbContext.Database.RollbackTransaction();
    }
}
