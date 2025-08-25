// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EndPointEcommerce.AdminPortal.Pages.Categories
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
