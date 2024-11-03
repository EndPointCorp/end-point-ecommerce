using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.AdminPortal.Services;

namespace EndPointCommerce.AdminPortal.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserSearcher _searcher;

        public IndexModel(IUserSearcher searcher)
        {
            _searcher = searcher;
        }

        public void OnGet() { }

        public async Task<JsonResult> OnGetSearchAsync(SearchParameters parameters)
        {
            var response = await _searcher.Search(parameters, new UrlBuilder(Url));

            return new JsonResult(response);
        }
    }
}
