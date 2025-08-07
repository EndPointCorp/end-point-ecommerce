using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.Services;

namespace EndPointEcommerce.AdminPortal.Pages.Quotes
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IQuoteSearcher _searcher;

        public IndexModel(IQuoteSearcher searcher)
        {
            _searcher = searcher;
        }

        public IList<Quote> Quotes { get; set; } = default!;

        public void OnGet() { }

        public async Task<JsonResult> OnGetSearchAsync(SearchParameters parameters)
        {
            var response = await _searcher.Search(parameters, new UrlBuilder(Url));

            return new JsonResult(response);
        }
    }
}
