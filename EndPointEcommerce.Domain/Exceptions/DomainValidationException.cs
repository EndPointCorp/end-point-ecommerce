using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Exceptions;

public class DomainValidationException : Exception
{
    public IEnumerable<ValidationResult> ValidationResults { get; set; } = [];

    public DomainValidationException() { }
    public DomainValidationException(string message) : base(message) { }
    public DomainValidationException(string message, Exception inner) : base(message, inner) { }
    public DomainValidationException(string message, IEnumerable<ValidationResult> validationResults) : base(message)
    {
        ValidationResults = validationResults;
    }

    public Dictionary<string, object?> ToDictionary()
    {
        var dictionary = new Dictionary<string, object?>
        {
            { "Message", Message }
        };

        foreach (var result in ValidationResults)
        {
            dictionary.Add(
                string.Join("_", result.MemberNames),
                result.ErrorMessage
            );
        }

        return dictionary;
    }
}
