// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.WebApi.Services;

namespace EndPointEcommerce.WebApi.Startup;

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
