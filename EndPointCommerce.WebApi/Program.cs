using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Configuration;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Startup;
using EndPointCommerce.WebApi.Startup;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace EndPointCommerce.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDataProtection().PersistKeysToFileSystem(
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
            opt.Password.RequiredLength = 8;
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.SignIn.RequireConfirmedEmail = false;
            opt.SignIn.RequireConfirmedPhoneNumber = false;
        }).AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<EndPointCommerceDbContext>();

        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

        builder.Services.AddDependencyInjectionServices();

        builder.Services.AddWebApiDependencyInjectionServices();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGroup("/api/User").MapIdentityApi<User>();

        app.MapControllers();

        app.Run();
    }
}
