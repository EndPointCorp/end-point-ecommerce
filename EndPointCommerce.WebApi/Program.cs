using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Configuration;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Startup;
using EndPointCommerce.WebApi.Startup;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace EndPointCommerce.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var origins = builder.Configuration["AllowedOrigins"]?.Split(",");
                if (origins == null || origins.Length == 0) return;

                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .AddDataProtection()
            .SetApplicationName("end-point-commerce-web-api")
            .PersistKeysToFileSystem(
                new DirectoryInfo(
                    builder.Configuration["WebApiDataProtectionKeysPath"] ??
                        throw new InvalidOperationException("Config setting 'WebApiDataProtectionKeysPath' not found.")
                )
            );

        builder.Services.AddEndPointCommerceDbContext(
            builder.Configuration.GetConnectionString("EndPointCommerceDbContext") ??
                throw new InvalidOperationException("Connection string 'EndPointCommerceDbContext' not found."),
            builder.Environment.IsDevelopment()
        );

        builder.Services.AddIdentityApiEndpoints<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
        }).AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<EndPointCommerceDbContext>();

        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

        builder.Services.AddDependencyInjectionServices();

        builder.Services.AddWebApiDependencyInjectionServices();

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
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        // app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthorization();

        app.MapGroup("/api/User").MapIdentityApi<User>();

        app.MapControllers();
        
        app.MapHealthChecks("/healthz");

        app.Run();
    }
}
