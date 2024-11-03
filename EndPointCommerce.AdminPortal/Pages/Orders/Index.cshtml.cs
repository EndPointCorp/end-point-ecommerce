using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.Services;

namespace EndPointCommerce.AdminPortal.Pages.Orders
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IOrderSearcher _searcher;

        public IndexModel(IOrderSearcher searcher)
        {
            _searcher = searcher;
        }

        public IList<Order> Orders { get; set; } = default!;

        public void OnGet() { }

        public async Task<JsonResult> OnGetSearchAsync(SearchParameters parameters)
        {
            var response = await _searcher.Search(parameters, new UrlBuilder(Url));

            return new JsonResult(response);
        }
    }
}
