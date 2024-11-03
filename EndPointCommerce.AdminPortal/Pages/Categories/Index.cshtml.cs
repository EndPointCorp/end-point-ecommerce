using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EndPointCommerce.AdminPortal.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICategoryRepository _repository;

        public IndexModel(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public IList<Category> Categories { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Categories = await _repository.FetchAllAsync();
        }
    }
}
