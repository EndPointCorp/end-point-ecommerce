// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Infrastructure.Startup;

// This class allows CLI tools to work.
// See https://github.com/dotnet/Scaffolding/issues/1765 and
// https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
public class EndPointEcommerceDbContextFactory : IDesignTimeDbContextFactory<EndPointEcommerceDbContext>
{
    public EndPointEcommerceDbContext CreateDbContext(string[] args)
    {
        var environment = GetEnvironment();
        var configuration = BuildConfiguration(environment);
        var connectionString = GetConnectionString(configuration);
        var options = BuildDbContextOptions(connectionString);

        return new EndPointEcommerceDbContext(options);
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
            // Optional config for local environment overrides, mainly useful during local development
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private string GetConnectionString(IConfigurationRoot configuration)
    {
        return configuration.GetConnectionString("EndPointEcommerceDbContext") ??
            throw new InvalidOperationException("Connection string 'EndPointEcommerceDbContext' not found.");
    }

    private DbContextOptions<EndPointEcommerceDbContext> BuildDbContextOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EndPointEcommerceDbContext>();
        optionsBuilder.BuildEndPointEcommerceDbContextOptions(connectionString);

        return optionsBuilder.Options;
    }
}
