using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.AdminPortal.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IIdentityService _identityService;

        public LogoutModel(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl)
        {
            await _identityService.LogoutAsync();
            return LocalRedirect(returnUrl ?? "/admin/");
        }
    }
}
