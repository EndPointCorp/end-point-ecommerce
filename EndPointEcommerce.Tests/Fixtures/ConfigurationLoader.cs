// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EndPointEcommerce.Tests.Fixtures;

public static class ConfigurationLoader
{
    public static IConfiguration LoadConfiguration()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                // Optional config for local environment overrides, mainly useful during local development
                builder.AddJsonFile("appsettings.Local.json",optional: true);
            })
            .Build();
        return host.Services.GetRequiredService<IConfiguration>();
    }
}
