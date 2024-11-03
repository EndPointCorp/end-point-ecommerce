using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.Services;

namespace EndPointCommerce.AdminPortal.Pages.Quotes
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
