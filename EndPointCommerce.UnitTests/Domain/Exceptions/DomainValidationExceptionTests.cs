using System.ComponentModel.DataAnnotations;
using EndPointCommerce.Domain.Exceptions;

namespace EndPointCommerce.UnitTests.Domain.Exceptions;

public class DomainValidationExceptionTest
{
    [Fact]
    public void ToDictionary_ShouldReturnADictionaryIncludingMessageAndValidationErrors()
    {
        // Arrange
        var validationResults = new List<ValidationResult>
        {
            new("test_message_01_02", ["test_member_01", "test_member_02"]),
            new("test_message_03_04", ["test_member_03", "test_member_04"])
        };
        var exception = new DomainValidationException("test_message", validationResults);

        // Act
        var result = exception.ToDictionary();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("Message"));
        Assert.Equal("test_message", result["Message"]);
        Assert.True(result.ContainsKey("test_member_01_test_member_02"));
        Assert.Equal("test_message_01_02", result["test_member_01_test_member_02"]);
        Assert.True(result.ContainsKey("test_member_03_test_member_04"));
        Assert.Equal("test_message_03_04", result["test_member_03_test_member_04"]);
    }
}