using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class UniqueCouponCodeAttribute : BaseUniqueFieldAttribute<ICouponRepository>
{
    protected override Coupon? FindRecord(ICouponRepository repository, object code) =>
        repository.FindByCodeAsync(code.ToString()!).Result;

    protected override string GetErrorMessage(object code) =>
        $"The coupon code '{code}' is already in use.";

    protected override bool IsSameRecord(object existing, object current) => 
        ((Coupon)current).Equals((Coupon)existing);
}
