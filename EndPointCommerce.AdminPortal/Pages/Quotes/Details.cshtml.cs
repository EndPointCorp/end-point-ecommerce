using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Quotes
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
