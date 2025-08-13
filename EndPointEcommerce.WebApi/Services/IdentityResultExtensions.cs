using Microsoft.AspNetCore.Identity;

namespace EndPointEcommerce.WebApi.Services;

public static class IdentityResultExtensions
{
    public static HttpValidationProblemDetails ToHttpValidationProblemDetails(this IdentityResult result) =>
        new()
        {
            Errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description }
            )
        };
}
