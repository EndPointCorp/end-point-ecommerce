using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Coupon repository interface.
/// </summary>
public interface ICouponRepository : IBaseRepository<Coupon>
{
    Task<Coupon?> FindByCodeAsync(string code);
}
