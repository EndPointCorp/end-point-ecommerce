using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class UniqueProductUrlKeyAttribute : BaseUniqueFieldAttribute<IProductRepository>
{
    protected override Product? FindRecord(IProductRepository repository, object urlKey) =>
        repository.FindByUrlKeyAsync(urlKey.ToString()!).Result;

    protected override string GetErrorMessage(object urlKey) =>
        $"The product URL Key '{urlKey}' is already in use.";

    protected override bool IsSameRecord(object existing, object current) => 
        ((Product)current).Equals((Product)existing);
}
