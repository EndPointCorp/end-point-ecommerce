using EndPointCommerce.Infrastructure.Startup;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.DataProtection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddDataProtection()
    .SetApplicationName("end-point-commerce-web-store")
    .PersistKeysToFileSystem(
        new DirectoryInfo(
            builder.Configuration["WebStoreDataProtectionKeysPath"] ??
                throw new InvalidOperationException("Config setting 'WebStoreDataProtectionKeysPath' not found.")
        )
    );

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".EndPointCommerce.WebStore.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddHttpClient<IApiClient, ApiClient>(client => {
    client.BaseAddress = new Uri(
        builder.Configuration["EndPointCommerceApiUrl"] ??
            throw new InvalidOperationException("Config setting 'EndPointCommerceApiUrl' not found.")
    );
});

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();
app.UseSerilogRequestLogging(opts =>
{
    opts.GetLevel = LogHelper.ExcludeHealthChecks;
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
