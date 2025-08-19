// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.AdminPortal.Services;

namespace EndPointEcommerce.AdminPortal.Startup;

public static class ServiceCollectionExtensions
{
    public static void AddAdminPortalDependencyInjectionServices(this IServiceCollection services)
    {
        services.AddScoped<IUserSearcher, UserSearcher>();
        services.AddScoped<ICustomerSearcher, CustomerSearcher>();
        services.AddScoped<ICouponSearcher, CouponSearcher>();
        services.AddScoped<IQuoteSearcher, QuoteSearcher>();
        services.AddScoped<IOrderSearcher, OrderSearcher>();
    }
}
