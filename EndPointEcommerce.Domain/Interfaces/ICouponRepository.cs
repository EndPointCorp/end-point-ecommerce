using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Coupon repository interface.
/// </summary>
public interface ICouponRepository : IBaseRepository<Coupon>
{
    Task<Coupon?> FindByCodeAsync(string code);
}
