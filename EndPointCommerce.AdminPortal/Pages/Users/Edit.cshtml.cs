using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IIdentityService _identityService;
        private readonly ICustomerRepository _customerRepository;

        public EditModel(
            IIdentityService identityService,
            ICustomerRepository customerRepository
        ) {
            _identityService = identityService;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public new UserViewModel User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _identityService.FindByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }
            User = await UserViewModel.FromModel(user, _identityService, _customerRepository);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index"));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(Page);
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            await User.FillRoles(_identityService);
            await User.FillCustomers(_customerRepository);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = User.ToModel();

            try
            {
                var result = await _identityService.UpdateAsync(user, User.Password, User.RoleName);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("User.Password", string.Join(" ", result.Errors.Select(x => x.Description)));
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _identityService.Exists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return onSuccess.Invoke();
        }
    }
}
