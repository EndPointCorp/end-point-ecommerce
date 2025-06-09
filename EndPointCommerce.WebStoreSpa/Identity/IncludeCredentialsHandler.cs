
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace EndPointCommerce.WebStoreSpa.Identity;

/// <summary>
/// Handler to ensure the "credentials" property is set to "include" in HTTP requests made in the browser.
/// This makes it so auth cookies and headers are sent with requests.
/// https://developer.mozilla.org/en-US/docs/Web/API/Request/credentials
/// </summary>
public class IncludeCredentialsHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    ) {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        // request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

        return base.SendAsync(request, cancellationToken);
    }
}
