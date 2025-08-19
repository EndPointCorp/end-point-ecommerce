// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.Services;

namespace EndPointEcommerce.AdminPortal.Pages.Orders
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
