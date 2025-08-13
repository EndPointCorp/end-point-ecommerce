using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Coupons
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ICouponRepository _couponRepository;

        public CreateModel(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        [BindProperty]
        public CouponViewModel Coupon { get; set; } = default!;

        public IActionResult OnGet()
        {
            Coupon = CouponViewModel.CreateDefault();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index"));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { Coupon.Id }));
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var coupon = Coupon.ToModel();

            await _couponRepository.AddAsync(coupon);

            Coupon.Id = coupon.Id;

            return onSuccess.Invoke();
        }
    }
}
