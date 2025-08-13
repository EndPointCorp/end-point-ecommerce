using Microsoft.AspNetCore.Http;

namespace EndPointEcommerce.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class AllFilesHaveImageFileExtensionAttribute : BaseFileValidationAttribute
{
    protected override string GetErrorMessage()
    {
        var allowedExtensions = string.Join(", ", imageExtensions);
        return $"Only the following file extensions are allowed: {allowedExtensions}.";
    }

    protected override bool CheckIfIsValid(object value)
    {
        var files = (IEnumerable<IFormFile>)value;
        return files.All(HasImageExtension);
    }

    protected override void ThrowIfTypeIsNotSupported(object value)
    {
        if (value is not IEnumerable<IFormFile>)
        {
            throw new ArgumentException($"{GetType().Name} only works with properties of type IEnumerable<IFormFile>.");
        }
    }
}
