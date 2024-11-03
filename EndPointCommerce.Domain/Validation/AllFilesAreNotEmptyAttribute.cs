using Microsoft.AspNetCore.Http;

namespace EndPointCommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class AllFilesAreNotEmptyAttribute : BaseFileValidationAttribute
{
    protected override string GetErrorMessage() =>
        "Some of the selected files appear to be empty.";

    protected override bool CheckIfIsValid(object value)
    {
        var files = (IEnumerable<IFormFile>)value;
        return files.All(HasLength);
    }

    protected override void ThrowIfTypeIsNotSupported(object value)
    {
        if (value is not IEnumerable<IFormFile>)
        {
            throw new ArgumentException($"{GetType().Name} only works with properties of type IEnumerable<IFormFile>.");
        }
    }
}
