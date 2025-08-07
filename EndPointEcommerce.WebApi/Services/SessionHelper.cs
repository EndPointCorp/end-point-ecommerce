using System.Security.Principal;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.WebApi.Services;

public interface ISessionHelper
{
    Task<int?> GetCustomerId(IPrincipal principal);
    bool IsAuthenticated(IPrincipal principal);
}

public class SessionHelper : ISessionHelper
{
    private readonly IIdentityService _identityService;

    public SessionHelper(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<int?> GetCustomerId(IPrincipal principal)
    {
        int? customerId = null;

        if (IsAuthenticated(principal))
        {
            var user = await _identityService.FindByUserNameAsync(principal.Identity!.Name!) ??
                throw new EntityNotFoundException();

            customerId = user.CustomerId;
        }

        return customerId;
    }

    public bool IsAuthenticated(IPrincipal principal) =>
        principal.Identity != null && principal.Identity.IsAuthenticated;
}