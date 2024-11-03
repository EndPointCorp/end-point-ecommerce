using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EndPointCommerce.Domain.Validation;

public abstract class BaseFileValidationAttribute : ValidationAttribute
{
    protected static readonly string[] imageExtensions = [".png", ".jpg", ".jpeg", ".gif", ".bmp"];

    protected abstract string GetErrorMessage();
    protected abstract bool CheckIfIsValid(object value);
    protected abstract void ThrowIfTypeIsNotSupported(object value);

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null) return ValidationResult.Success;

        ThrowIfTypeIsNotSupported(value);

        if (!CheckIfIsValid(value)) return new ValidationResult(GetErrorMessage());

        return ValidationResult.Success;
    }

    protected static bool HasLength(IFormFile file) => file.Length > 0;

    protected static bool HasImageExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !imageExtensions.Contains(extension))
        {
            return false;
        }

        return true;
    }
}