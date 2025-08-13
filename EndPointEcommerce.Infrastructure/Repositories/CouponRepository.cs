using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Coupon entity.
/// </summary>
public class CouponRepository : BaseAuditRepository<Coupon>, ICouponRepository
{
    public CouponRepository(EndPointEcommerceDbContext context, IHttpContextAccessor httpContextAccessor) :
        base(context, httpContextAccessor) { }

    public async Task<Coupon?> FindByCodeAsync(string code)
    {
        return await DbSet()
            .FirstOrDefaultAsync(c => c.Code.Trim().ToLower().Equals(code.Trim().ToLower()));
    }
}