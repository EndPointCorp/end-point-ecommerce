using EndPointCommerce.WebApi.Services;

namespace EndPointCommerce.WebApi.Startup;

public static class ServiceCollectionExtensions
{
    public static void AddWebApiDependencyInjectionServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionHelper, SessionHelper>();
        services.AddScoped<IQuoteResolver, QuoteResolver>();
        services.AddScoped<IDataProtectorProxy, DataProtectorProxy>();
        services.AddScoped<IQuoteCookieManager, QuoteCookieManager>();
    }
}
