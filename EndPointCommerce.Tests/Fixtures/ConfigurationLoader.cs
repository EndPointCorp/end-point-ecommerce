using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EndPointCommerce.Tests.Fixtures;

public static class ConfigurationLoader
{
    public static IConfiguration LoadConfiguration()
    {
        var host = Host.CreateDefaultBuilder().Build();
        return host.Services.GetRequiredService<IConfiguration>();
    }
}
