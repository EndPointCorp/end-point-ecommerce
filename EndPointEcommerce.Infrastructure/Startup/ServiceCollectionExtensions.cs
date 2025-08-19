// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.Infrastructure.Data;
using EndPointEcommerce.Infrastructure.Repositories;
using EndPointEcommerce.Infrastructure.Services;
using EndPointEcommerce.Infrastructure.Services.Payments;
using EndPointEcommerce.RazorTemplates.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace EndPointEcommerce.Infrastructure.Startup;

public static class ServiceCollectionExtensions
{
    public static void AddEndPointEcommerceDbContext(this IServiceCollection services, string connectionString, bool isDevelopment = false)
    {
        services.AddDbContext<EndPointEcommerceDbContext>(options =>
            options.BuildEndPointEcommerceDbContextOptions(connectionString, isDevelopment)
        );
    }

    public static void AddDependencyInjectionServices(this IServiceCollection services)
    {
        services.AddScoped<ITaxCalculator, TaxJarTaxCalculator>();
        services.AddScoped<IPaymentGateway, AuthorizeNetPaymentGateway>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IQuoteItemRepository, QuoteItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICategoryCreator, CategoryCreator>();
        services.AddScoped<ICategoryUpdater, CategoryUpdater>();
        services.AddScoped<ICategoryMainImageDeleter, CategoryMainImageDeleter>();
        services.AddScoped<IProductCreator, ProductCreator>();
        services.AddScoped<IProductUpdater, ProductUpdater>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductMainImageDeleter, ProductMainImageDeleter>();
        services.AddScoped<IProductThumbnailImageDeleter, ProductThumbnailImageDeleter>();
        services.AddScoped<IProductAdditionalImageDeleter, ProductAdditionalImageDeleter>();
        services.AddScoped<IQuoteUpdater, QuoteUpdater>();
        services.AddScoped<IQuoteTaxCalculator, QuoteTaxCalculator>();
        services.AddScoped<IQuoteItemCreator, QuoteItemCreator>();
        services.AddScoped<IQuoteItemUpdater, QuoteItemUpdater>();
        services.AddScoped<IQuoteItemDeleter, QuoteItemDeleter>();
        services.AddScoped<IQuoteValidator, QuoteValidator>();
        services.AddScoped<IOrderCreator, OrderCreator>();

        services.AddTransient<ISmtpClient, SmtpClient>();
        services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();
        services.AddTransient<IMailer, Mailer>();
        services.AddTransient<Microsoft.AspNetCore.Identity.IEmailSender<User>, IdentityEmailSender>();
        services.AddScoped<IOrderConfirmationMailer, OrderConfirmationMailer>();
    }
}