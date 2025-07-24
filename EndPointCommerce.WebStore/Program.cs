using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EndPointCommerce.WebStore;
using EndPointCommerce.WebStore.Identity;
using EndPointCommerce.WebStore.Api;
using EndPointCommerce.WebStore.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HACK: Slightly hacky way of loading appsettings.Local.json files like is done with the other
//       projects. WebAssemblyHostBuilder.InitializeEnvironment() is internally doing some
//       filesystem-based detection of what appsettings files it loads by default, but using the
//       same methods here doesn't work at all!? I'm not sure what magic is going on there.
//       So, we'll do this as a last resort ... We only load it in Development builds because
//       in a real-world production environment, you probably don't want to see random requests
//       to "/appsettings.Local.json" failing with a 404.
if (builder.HostEnvironment.IsDevelopment())
{
    using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    var response = await httpClient.GetAsync("appsettings.Local.json");
    if (response.IsSuccessStatusCode)
    {
        var json = await response.Content.ReadAsStringAsync();
        var config = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();
        builder.Configuration.AddConfiguration(config);
    }
}

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
