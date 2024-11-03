using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Startup;

public static class DbContextOptionsBuilderExtensions
{
    public static void BuildEndPointCommerceDbContextOptions(
        this DbContextOptionsBuilder options,
        string connectionString,
        bool isDevelopment = false
    ) {
        options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        if (isDevelopment)
        {
            options
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
}