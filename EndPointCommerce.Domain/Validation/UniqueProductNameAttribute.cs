using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class UniqueProductNameAttribute : BaseUniqueFieldAttribute<IProductRepository>
{
    protected override Product? FindRecord(IProductRepository repository, object name) =>
        repository.FindByNameAsync(name.ToString()!).Result;

    protected override string GetErrorMessage(object name) =>
        $"The product name '{name}' is already in use.";

    protected override bool IsSameRecord(object existing, object current) => 
        ((Product)current).Equals((Product)existing);
}
