using EndPointCommerce.AdminPortal.Startup;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Startup;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace EndPointCommerce.AdminPortal;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddDataProtection()
            .SetApplicationName("end-point-commerce-admin-portal")
            .PersistKeysToFileSystem(
                new DirectoryInfo(
                    builder.Configuration["AdminPortalDataProtectionKeysPath"] ??
                        throw new InvalidOperationException("Config setting 'AdminPortalDataProtectionKeysPath' not found.")
                )
            );

        builder.Services.AddEndPointCommerceDbContext(
            builder.Configuration.GetConnectionString("EndPointCommerceDbContext") ??
                throw new InvalidOperationException("Connection string 'EndPointCommerceDbContext' not found."),
            builder.Environment.IsDevelopment()
        );

        builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.SignIn.RequireConfirmedEmail = false;
        }).AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<EndPointCommerceDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager<SignInManager<User>>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = new PathString("/admin/Account/AccessDenied");
            options.LoginPath = new PathString("/admin/Account/Login");
            options.LogoutPath = new PathString("/admin/Account/Logout");
        });

        // Bug: https://stackoverflow.com/questions/77970596/how-to-register-system-timeprovider-implementation
        builder.Services.AddSingleton(_ => TimeProvider.System);

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication().AddCookie("Identity.Application");

        builder.Services.AddDependencyInjectionServices();

        builder.Services.AddAdminPortalDependencyInjectionServices();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<EndPointCommerceDbContext>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration)
        );

        var app = builder.Build();

        app.UseRequestLocalization("en-US");

        app.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = LogHelper.ExcludeHealthChecks;
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // app.UseHsts();
        }

        app.UsePathBase("/admin");

        // app.UseHttpsRedirection();

        app.UseStaticFiles();

        var categoryImagesPath = builder.Configuration["CategoryImagesPath"] ??
            throw new InvalidOperationException("Config setting 'CategoryImagesPath' not found.");

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(categoryImagesPath),
            RequestPath = new PathString("/category-images"),
        });

        var productImagesPath = builder.Configuration["ProductImagesPath"] ??
            throw new InvalidOperationException("Config setting 'ProductImagesPath' not found.");

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(productImagesPath),
            RequestPath = new PathString("/product-images"),
        });

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.MapHealthChecks("/healthz");

        app.Run();
    }
}
