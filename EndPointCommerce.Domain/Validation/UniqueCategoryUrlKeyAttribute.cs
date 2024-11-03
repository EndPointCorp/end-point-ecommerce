using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class UniqueCategoryUrlKeyAttribute : BaseUniqueFieldAttribute<ICategoryRepository>
{
    protected override Category? FindRecord(ICategoryRepository repository, object urlKey) =>
        repository.FindByUrlKeyAsync(urlKey.ToString()!).Result;

    protected override string GetErrorMessage(object urlKey) =>
        $"The category URL Key '{urlKey}' is already in use.";

    protected override bool IsSameRecord(object existing, object current) => 
        ((Category)current).Equals((Category)existing);
}
