using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Coupon entity.
/// </summary>
public class CouponRepository : BaseAuditRepository<Coupon>, ICouponRepository
{
    public CouponRepository(EndPointCommerceDbContext context, IHttpContextAccessor httpContextAccessor) :
        base(context, httpContextAccessor) { }

    public async Task<Coupon?> FindByCodeAsync(string code)
    {
        return await DbSet()
            .FirstOrDefaultAsync(c => c.Code.Trim().ToLower().Equals(code.Trim().ToLower()));
    }
}