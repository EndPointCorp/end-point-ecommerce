using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Infrastructure.Data;
using EndPointEcommerce.Infrastructure.Startup;
using EndPointEcommerce.Jobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Optional config for local environment overrides, mainly useful during local development
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

builder.Services
    .AddDataProtection()
    .SetApplicationName("end-point-ecommerce-jobs")
    .UseEphemeralDataProtectionProvider();

builder.Services.AddEndPointEcommerceDbContext(
    builder.Configuration.GetConnectionString("EndPointEcommerceDbContext") ??
        throw new InvalidOperationException("Connection string 'EndPointEcommerceDbContext' not found."),
    builder.Environment.IsDevelopment()
);

builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = false;

}).AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<EndPointEcommerceDbContext>();

builder.Services.AddDependencyInjectionServices();

var host = builder.Build();

return await Cli.Run(args, host);
