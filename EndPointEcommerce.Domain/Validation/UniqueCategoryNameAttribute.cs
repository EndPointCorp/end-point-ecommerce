// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class UniqueCategoryNameAttribute : BaseUniqueFieldAttribute<ICategoryRepository>
{
    protected override Category? FindRecord(ICategoryRepository repository, object name) =>
        repository.FindByNameAsync(name.ToString()!).Result;

    protected override string GetErrorMessage(object name) =>
        $"The category name '{name}' is already in use.";

    protected override bool IsSameRecord(object existing, object current) => 
        ((Category)current).Equals((Category)existing);
}
