using EndPointCommerce.Infrastructure.Data;

namespace EndPointCommerce.Tests.Fixtures;

/// <summary>
/// Wraps all test cases within a database transaction that is rolled back when the test case finishes.
/// </summary>
public abstract class TransactionalTests : IClassFixture<DatabaseFixture>, IDisposable
{
    protected readonly EndPointCommerceDbContext dbContext;

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
