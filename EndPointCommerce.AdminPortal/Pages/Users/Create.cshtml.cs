using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IIdentityService _identityService;
        private readonly ICustomerRepository _customerRepository;

        public CreateModel(
            IIdentityService identityService,
            ICustomerRepository customerRepository
        ) {
            _identityService = identityService;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public new UserViewModel User { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            User = await UserViewModel.CreateDefault(_identityService, _customerRepository);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index"));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { User.Id }));
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

            var result = await _identityService.AddAsync(user, User.Password ?? "", User.RoleName);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("User.Password", string.Join(" ", result.Errors.Select(x => x.Description)));
                return Page();
            }

            User.Id = user.Id;

            return onSuccess.Invoke();
        }
    }
}
