using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace EndPointEcommerce.WebStore.Identity;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public ApiAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("EndPointEcommerce.WebApi");
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var response = await _httpClient.GetAsync("/api/User/manage/info");
        if (!response.IsSuccessStatusCode) return BuildUnauthenticatedState();

        var json = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<UserInfo>(json, camelCase);
        if (userInfo is null) return BuildUnauthenticatedState();

        return BuildAuthenticatedState(userInfo.Email);
    }

    public async Task<bool> IsUserAuthenticated()
    {
        var authState = await GetAuthenticationStateAsync();
        return authState.User.Identity?.IsAuthenticated ?? false;
    }

    public void UpdateAuthenticationState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private record UserInfo(string Email);

    private AuthenticationState BuildAuthenticatedState(string email) =>
        new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new(ClaimTypes.Name, email),
                        new(ClaimTypes.Email, email)
                    ],
                    nameof(ApiAuthenticationStateProvider)
                )
            )
        );

    private AuthenticationState BuildUnauthenticatedState() =>
        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly JsonSerializerOptions camelCase =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}
