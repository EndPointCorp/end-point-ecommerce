using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Infrastructure.Startup;

// This class allows CLI tools to work.
// See https://github.com/dotnet/Scaffolding/issues/1765 and
// https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
public class EndPointCommerceDbContextFactory : IDesignTimeDbContextFactory<EndPointCommerceDbContext>
{
    public EndPointCommerceDbContext CreateDbContext(string[] args)
    {
        var environment = GetEnvironment();
        var configuration = BuildConfiguration(environment);
        var connectionString = GetConnectionString(configuration);
        var options = BuildDbContextOptions(connectionString);

        return new EndPointCommerceDbContext(options);
    }

    private string? GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    private IConfigurationRoot BuildConfiguration(string? environment)
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private string GetConnectionString(IConfigurationRoot configuration)
    {
        return configuration.GetConnectionString("EndPointCommerceDbContext") ??
            throw new InvalidOperationException("Connection string 'EndPointCommerceDbContext' not found.");
    }

    private DbContextOptions<EndPointCommerceDbContext> BuildDbContextOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EndPointCommerceDbContext>();
        optionsBuilder.BuildEndPointCommerceDbContextOptions(connectionString);

        return optionsBuilder.Options;
    }
}
