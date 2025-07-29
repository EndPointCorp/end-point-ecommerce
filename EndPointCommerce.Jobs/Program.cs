using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Startup;
using EndPointCommerce.Jobs;
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
    .SetApplicationName("end-point-commerce-jobs")
    .UseEphemeralDataProtectionProvider();

builder.Services.AddEndPointCommerceDbContext(
    builder.Configuration.GetConnectionString("EndPointCommerceDbContext") ??
        throw new InvalidOperationException("Connection string 'EndPointCommerceDbContext' not found."),
    builder.Environment.IsDevelopment()
);

builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = false;

}).AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<EndPointCommerceDbContext>();

builder.Services.AddDependencyInjectionServices();

var host = builder.Build();

return await Cli.Run(args, host);
