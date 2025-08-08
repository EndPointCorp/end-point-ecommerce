
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace EndPointEcommerce.WebStore.Api;

/// <summary>
/// Handler to ensure the "credentials" property is set to "include" in HTTP requests made in the browser.
/// This makes it so cookies and headers are sent with requests.
/// https://developer.mozilla.org/en-US/docs/Web/API/Request/credentials
/// </summary>
public class IncludeCredentialsHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    ) {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return base.SendAsync(request, cancellationToken);
    }
}
