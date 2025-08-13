using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Coupons
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ICouponRepository _couponRepository;

        public EditModel(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        [BindProperty]
        public CouponViewModel Coupon { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _couponRepository.FindByIdAsync(id.Value);
            if (coupon == null)
            {
                return NotFound();
            }
            Coupon = CouponViewModel.FromModel(coupon);
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

            try
            {
                await _couponRepository.UpdateAsync(Coupon.ToModel());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _couponRepository.Exists(Coupon.Id))
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
