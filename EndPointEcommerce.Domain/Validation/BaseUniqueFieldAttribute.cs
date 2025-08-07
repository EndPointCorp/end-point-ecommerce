using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace EndPointEcommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public abstract class BaseUniqueFieldAttribute<TRepo> : ValidationAttribute
{
    protected abstract object? FindRecord(TRepo repository, object value);
    protected abstract string GetErrorMessage(object value);
    protected abstract bool IsSameRecord(object existing, object current);

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null) return ValidationResult.Success;

        var repository = GetRepository(context);
        if (repository == null) return ValidationResult.Success;

        return Validate(value, context.ObjectInstance, repository);
    }

    private ValidationResult? Validate(object value, object current, TRepo repository)
    {
        var existing = FindRecord(repository, value);

        if (existing != null && !IsSameRecord(existing, current))
        {
            return new ValidationResult(GetErrorMessage(value));
        }

        return ValidationResult.Success;
    }

    private static TRepo? GetRepository(ValidationContext validationContext)
    {
        return validationContext.GetService<TRepo>();
    }
}
