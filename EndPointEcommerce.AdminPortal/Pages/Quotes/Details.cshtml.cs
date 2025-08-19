// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Quotes
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly IQuoteRepository _quoteRepository;

        public DetailsModel(IQuoteRepository quoteRepository)
        {
            _quoteRepository = quoteRepository;
        }

        [BindProperty]
        public QuoteViewModel Quote { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quote = await _quoteRepository.FindByIdAsync(id.Value);
            if (quote == null)
            {
                return NotFound();
            }

            Quote = QuoteViewModel.FromModel(quote);
            return Page();
        }
    }
}
