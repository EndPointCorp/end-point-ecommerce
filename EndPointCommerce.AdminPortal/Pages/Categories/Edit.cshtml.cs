using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ICategoryUpdater _categoryUpdater;
        private readonly ICategoryMainImageDeleter _categoryMainImageDeleter;
        private readonly ICategoryRepository _repository;

        public EditModel(
            ICategoryUpdater categoryUpdater,
            ICategoryMainImageDeleter categoryMainImageDeleter,
            ICategoryRepository repository
        )
        {
            _categoryUpdater = categoryUpdater;
            _categoryMainImageDeleter = categoryMainImageDeleter;
            _repository = repository;
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var category = await _repository.FindByIdAsync(id.Value);
            if (category == null) return NotFound();

            Category = CategoryViewModel.FromModel(category);

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

        public async Task<IActionResult> OnPostDeleteMainImageAsync()
        {
            try
            {
                var result = await _categoryMainImageDeleter.Run(Category.Id);
                Category = CategoryViewModel.FromModel(result);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

            return Page();
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _categoryUpdater.Run(Category.ToInputPayload());

                Category = CategoryViewModel.FromModel(result);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

            return onSuccess.Invoke();
        }
    }
}
