using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EndPointCommerce.Infrastructure.Data;

namespace EndPointCommerce.IntegrationTests.Fixtures;

// https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database#creating-seeding-and-managing-a-test-database
public class DatabaseFixture
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public DatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var dbContext = CreateDbContext())
                {
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                }

                _databaseInitialized = true;
            }
        }
    }

    public EndPointCommerceDbContext CreateDbContext()
    {
        var host = Host.CreateDefaultBuilder().Build();
        var config = host.Services.GetRequiredService<IConfiguration>();

        var options = new DbContextOptionsBuilder<EndPointCommerceDbContext>()
            .UseNpgsql(config.GetConnectionString("EndPointCommerceDbContext"))
            .UseSnakeCaseNamingConvention()
            .Options;

        var dbContext = new EndPointCommerceDbContext(options);

        return dbContext;
    }

    public async Task WithTransaction(EndPointCommerceDbContext dbContext, Func<Task> test)
    {
        dbContext.Database.BeginTransaction();

        try
        {
            await test.Invoke();
        }
        catch
        {
            throw;
        }
        finally
        {
            dbContext.Database.RollbackTransaction();
        }
    }
}
