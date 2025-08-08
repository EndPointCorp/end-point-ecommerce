using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointEcommerce.Infrastructure.Data;
using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EndPointEcommerce.AdminPortal.Pages.Addresses
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly EndPointEcommerceDbContext _context;

        public IndexModel(EndPointEcommerceDbContext context)
        {
            _context = context;
        }

        public IList<Address> Addresses { get; set; } = default!;
        public int? CustomerId { get; set; } = default!;

        public async Task OnGetAsync(int? customerId)
        {
            CustomerId = customerId;

            var query = _context.Addresses as IQueryable<Address>;

            if (customerId != null)
                query = query.Where(x => x.Customer!.Id == customerId);

            Addresses = await query
                .Include(x => x.Country)
                .Include(x => x.State)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }
    }
}
