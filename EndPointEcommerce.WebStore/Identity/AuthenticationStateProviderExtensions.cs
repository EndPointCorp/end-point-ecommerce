// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Components.Authorization;

namespace EndPointEcommerce.WebStore.Identity;

public static class AuthenticationStateProviderExtensions
{
    public static async Task<bool> IsUserAuthenticated(this AuthenticationStateProvider provider) =>
        await ((ApiAuthenticationStateProvider)provider).IsUserAuthenticated();

    public static void UpdateAuthenticationState(this AuthenticationStateProvider provider) =>
        ((ApiAuthenticationStateProvider)provider).UpdateAuthenticationState();
}
