using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Infrastructure.Data;

namespace EndPointEcommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteContentsController : ControllerBase
    {
        private readonly EndPointEcommerceDbContext _context;

        public SiteContentsController(EndPointEcommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/SiteContents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteContent>>> GetSiteContents()
        {
            return await _context.SiteContents.OrderBy(x => x.Name).ToListAsync();
        }
    }
}
