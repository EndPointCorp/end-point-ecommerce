using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IProductCreator _productCreator;
        private readonly ICategoryRepository _categoryRepository;

        public CreateModel(
            IProductCreator productCreator,
            ICategoryRepository categoryRepository
        ) {
            _productCreator = productCreator;
            _categoryRepository = categoryRepository;
        }

        [BindProperty]
        public ProductViewModel Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Product = ProductViewModel.CreateDefault();
            await Product.PopulateCategories(_categoryRepository);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index"));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { Product.Id }));
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            if (!ModelState.IsValid)
            {
                await Product.PopulateCategories(_categoryRepository);
                return Page();
            }

            var result = await _productCreator.Run(Product.ToInputPayload());
            Product = ProductViewModel.FromModel(result);

            return onSuccess.Invoke();
        }
    }
}
