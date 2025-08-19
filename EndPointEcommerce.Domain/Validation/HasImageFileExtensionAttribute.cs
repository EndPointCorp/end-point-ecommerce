// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Http;

namespace EndPointEcommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class HasImageFileExtensionAttribute : BaseFileValidationAttribute
{
    protected override string GetErrorMessage()
    {
        var allowedExtensions = string.Join(", ", imageExtensions);
        return $"Only the following file extensions are allowed: {allowedExtensions}.";
    }

    protected override bool CheckIfIsValid(object value)
    {
        var file = (IFormFile)value;
        return HasImageExtension(file);
    }

    protected override void ThrowIfTypeIsNotSupported(object value)
    {
        if (value is not IFormFile)
        {
            throw new ArgumentException($"{GetType().Name} only works with properties of type IFormFile.");
        }
    }
}
