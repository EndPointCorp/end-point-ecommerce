using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EndPointCommerce.WebStoreSpa;
using EndPointCommerce.WebStoreSpa.Identity;
using EndPointCommerce.WebStoreSpa.Api;
using EndPointCommerce.WebStoreSpa.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddTransient<IncludeCredentialsHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient(
    "EndPointCommerce.WebApi",
    opt => opt.BaseAddress = new Uri(
        builder.Configuration["EndPointCommerceApiUrl"] ??
            throw new InvalidOperationException("Config setting 'EndPointCommerceApiUrl' not found.")
    )
).AddHttpMessageHandler<IncludeCredentialsHandler>();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddSingleton<QuoteStateContainer>();
builder.Services.AddSingleton<AlertStateContainer>();

await builder.Build().RunAsync();
