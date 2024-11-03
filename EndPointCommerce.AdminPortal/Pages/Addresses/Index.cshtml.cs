using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EndPointCommerce.AdminPortal.Pages.Addresses
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly EndPointCommerceDbContext _context;

        public IndexModel(EndPointCommerceDbContext context)
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
            Addresses = await query.Include(x => x.State).OrderByDescending(x => x.Id).ToListAsync();
        }
    }
}
