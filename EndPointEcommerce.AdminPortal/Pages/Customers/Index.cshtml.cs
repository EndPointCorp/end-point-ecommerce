using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EndPointEcommerce.AdminPortal.Services;

namespace EndPointEcommerce.AdminPortal.Pages.Customers
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICustomerSearcher _searcher;

        public IndexModel(ICustomerSearcher searcher)
        {
            _searcher = searcher;
        }

        public IList<Customer> Customers { get; set; } = default!;

        public void OnGet() { }

        public async Task<JsonResult> OnGetSearchAsync(SearchParameters parameters)
        {
            var response = await _searcher.Search(parameters, new UrlBuilder(Url));

            return new JsonResult(response);
        }
    }
}
