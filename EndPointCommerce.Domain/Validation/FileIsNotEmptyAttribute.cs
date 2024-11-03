using Microsoft.AspNetCore.Http;

namespace EndPointCommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class FileIsNotEmptyAttribute : BaseFileValidationAttribute
{
    protected override string GetErrorMessage()
    {
        return "The selected file appears to be empty.";
    }

    protected override bool CheckIfIsValid(object value)
    {
        var file = (IFormFile)value;
        return HasLength(file);
    }

    protected override void ThrowIfTypeIsNotSupported(object value)
    {
        if (value is not IFormFile)
        {
            throw new ArgumentException($"{GetType().Name} only works with properties of type IFormFile.");
        }
    }
}
