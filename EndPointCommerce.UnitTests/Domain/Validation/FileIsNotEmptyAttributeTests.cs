using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using EndPointCommerce.Domain.Validation;
using Moq;

namespace EndPointCommerce.UnitTests.Domain.Validation;

public class FileIsNotEmptyAttributeTests
{
    private static FileIsNotEmptyAttribute BuildTestSubject() => new();

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenTheFieldThatTheAttributeIsAppliedToIsNull()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ThrowsAnException_WhenTheAttributeIsAppliedToAPropertyOfATypeThatIsNotSupported()
    {
        // Arrange
        var attribute = BuildTestSubject();
        object value = new();
        var context = new ValidationContext(new object());

        // Actv & Assert
        var exception = Assert.Throws<ArgumentException>(() => attribute.GetValidationResult(value, context));

        Assert.Equal("FileIsNotEmptyAttribute only works with properties of type IFormFile.", exception.Message);
    }

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenANonEmptyFileIsGiven()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var file = new Mock<IFormFile>();
        file.Setup(f => f.Length).Returns(1);

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(file.Object, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAnEmptyFileIsGiven()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var file = new Mock<IFormFile>();
        file.Setup(f => f.Length).Returns(0);

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(file.Object, context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("The selected file appears to be empty.", result?.ErrorMessage);
    }
}