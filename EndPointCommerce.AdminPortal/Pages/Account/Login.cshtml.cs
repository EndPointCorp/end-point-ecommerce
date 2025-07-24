using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly IIdentityService _identityService;

        public LoginModel(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [BindProperty]
        public LoginViewModel Login { get; set; } = default!;

        public IActionResult OnGet()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return RedirectAfterLogin();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            if (!ModelState.IsValid) return Page();

            var user = await _identityService.FindByUserNameAsync(Login.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var isPasswordValid = await _identityService.IsPasswordValid(user, Login.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var role = await _identityService.GetRoleAsync(user);

            if (role.Name != Domain.Entities.User.ADMIN_ROLE)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var result = await _identityService.LoginAsync(Login.Email!, Login.Password);

            if (result.Succeeded)
            {
                return RedirectAfterLogin(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is locked.");
                return Page();
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        protected IActionResult RedirectAfterLogin(string? returnUrl = null)
        {
            var defaultRedirect = "/";
            return LocalRedirect(returnUrl ?? defaultRedirect);
        }
    }
}
