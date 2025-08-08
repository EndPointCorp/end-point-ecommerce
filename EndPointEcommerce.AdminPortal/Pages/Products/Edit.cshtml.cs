using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IProductUpdater _productUpdater;
        private readonly IProductMainImageDeleter _productMainImageDeleter;
        private readonly IProductThumbnailImageDeleter _productThumbnailImageDeleter;
        private readonly IProductAdditionalImageDeleter _productAdditionalImageDeleter;
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;

        public EditModel(
            IProductUpdater productUpdater,
            IProductMainImageDeleter productMainImageDeleter,
            IProductThumbnailImageDeleter productThumbnailImageDeleter,
            IProductAdditionalImageDeleter productAdditionalImageDeleter,
            IProductRepository repository,
            ICategoryRepository categoryRepository
        ) {
            _productUpdater = productUpdater;
            _productMainImageDeleter = productMainImageDeleter;
            _productThumbnailImageDeleter = productThumbnailImageDeleter;
            _productAdditionalImageDeleter = productAdditionalImageDeleter;
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        [BindProperty]
        public ProductViewModel Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var product = await _repository.FindByIdAsync(id.Value);
            if (product == null) return NotFound();

            Product = ProductViewModel.FromModel(product);
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

        public async Task<IActionResult> OnPostDeleteMainImageAsync()
        {
            return await HandleDeleteImage(
                async () => await _productMainImageDeleter.Run(Product.Id)
            );
        }

        public async Task<IActionResult> OnPostDeleteThumbnailImageAsync()
        {
            return await HandleDeleteImage(
                async () => await _productThumbnailImageDeleter.Run(Product.Id)
            );
        }

        public async Task<IActionResult> OnPostDeleteAdditionalImageAsync(int imageId)
        {
            return await HandleDeleteImage(
                async () => await _productAdditionalImageDeleter.Run(Product.Id, imageId)
            );
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            if (!ModelState.IsValid)
            {
                await Product.PopulateImages(_repository);
                await Product.PopulateCategories(_categoryRepository);
                return Page();
            }

            try
            {
                var result = await _productUpdater.Run(Product.ToInputPayload());
                Product = ProductViewModel.FromModel(result);
                await Product.PopulateCategories(_categoryRepository);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

            return onSuccess.Invoke();
        }

        private async Task<IActionResult> HandleDeleteImage(Func<Task<Product>> runDeleter)
        {
            try
            {
                var result = await runDeleter();
                Product = ProductViewModel.FromModel(result);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

            await Product.PopulateCategories(_categoryRepository);
            return Page();
        }
    }
}
