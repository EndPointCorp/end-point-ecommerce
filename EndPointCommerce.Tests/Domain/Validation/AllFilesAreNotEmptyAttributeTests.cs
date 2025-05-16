using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using EndPointCommerce.Domain.Validation;
using Moq;

namespace EndPointCommerce.Tests.Domain.Validation;

public class AllFilesAreNotEmptyAttributeTests
{
    private static AllFilesAreNotEmptyAttribute BuildTestSubject() => new();

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

        Assert.Equal("AllFilesAreNotEmptyAttribute only works with properties of type IEnumerable<IFormFile>.", exception.Message);
    }

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenNonEmptyFilesAreGiven()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var file_1 = new Mock<IFormFile>();
        file_1.Setup(f => f.Length).Returns(1);

        var file_2 = new Mock<IFormFile>();
        file_2.Setup(f => f.Length).Returns(2);

        object files = new List<IFormFile>() { file_1.Object, file_2.Object };

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(files, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAnEmptyFileIsGiven()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var file_1 = new Mock<IFormFile>();
        file_1.Setup(f => f.Length).Returns(1);

        var file_2 = new Mock<IFormFile>();
        file_2.Setup(f => f.Length).Returns(0);

        object files = new List<IFormFile>() { file_1.Object, file_2.Object };

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(files, context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Some of the selected files appear to be empty.", result?.ErrorMessage);
    }
}