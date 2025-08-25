// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ICategoryCreator _categoryCreator;

        public CreateModel(ICategoryCreator categoryCreator)
        {
            _categoryCreator = categoryCreator;
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index"));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { Category.Id }));
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _categoryCreator.Run(Category.ToInputPayload());

            Category = CategoryViewModel.FromModel(result);

            return onSuccess.Invoke();
        }
    }
}
