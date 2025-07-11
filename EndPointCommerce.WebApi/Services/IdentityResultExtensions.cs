using Microsoft.AspNetCore.Identity;

namespace EndPointCommerce.WebApi.Services;

public static class IdentityResultExtensions
{
    public static HttpValidationProblemDetails ToHttpValidationProblemDetails(this IdentityResult result)
    {
        return new HttpValidationProblemDetails
        {
            Errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description }
            )
        };
    } 
}
