// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EndPointEcommerce.AdminPortal.Pages.SiteContents
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly EndPointEcommerceDbContext _context;

        public IndexModel(EndPointEcommerceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IList<SiteContent> SiteContents { get; set; } = default!;

        public async Task OnGetAsync()
        {
            SiteContents = await _context.SiteContents.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            foreach (var vm in SiteContents)
            {
                var contentToUpdate = await _context.SiteContents.SingleAsync(c => c.Id == vm.Id);
                contentToUpdate.Content = vm.Content;

                _context.Update(contentToUpdate);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
