using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteContentsController : ControllerBase
    {
        private readonly EndPointCommerceDbContext _context;

        public SiteContentsController(EndPointCommerceDbContext context)
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
