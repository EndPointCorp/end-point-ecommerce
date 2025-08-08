using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointEcommerce.AdminPortal.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {

        public void OnGet()
        {
        }
        public IActionResult OnPostAsync()
        {
            return RedirectToPage("/Account/Logout");
        }

    }
}
