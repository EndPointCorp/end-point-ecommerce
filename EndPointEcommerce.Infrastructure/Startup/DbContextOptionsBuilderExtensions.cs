// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Startup;

public static class DbContextOptionsBuilderExtensions
{
    public static void BuildEndPointEcommerceDbContextOptions(
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