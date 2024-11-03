using EndPointCommerce.AdminPortal.Services;

namespace EndPointCommerce.AdminPortal.Startup;

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
