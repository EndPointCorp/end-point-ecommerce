using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EndPointCommerce.Infrastructure.Data;

namespace EndPointCommerce.Tests.Fixtures;

/// <summary>
/// Test fixture that allows test cases to access a test database.
/// https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database#creating-seeding-and-managing-a-test-database
/// </summary>
public class DatabaseFixture
{
    private static readonly Lock _lock = new();
    private static bool _databaseInitialized;

    public IConfiguration Configuration { get; private set; }

    public DatabaseFixture()
    {
        Configuration = ConfigurationLoader.LoadConfiguration();

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
        var options = new DbContextOptionsBuilder<EndPointCommerceDbContext>()
            .UseNpgsql(Configuration.GetConnectionString("EndPointCommerceDbContext"))
            .UseSnakeCaseNamingConvention()
            // Useful for debugging. Uncomment for more verbose logs.
            // .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            // .EnableSensitiveDataLogging()
            .Options;

        var dbContext = new EndPointCommerceDbContext(options);

        return dbContext;
    }
}
