using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using EndPointCommerce.Domain.Validation;
using Moq;

namespace EndPointCommerce.Tests.Domain.Validation;

public class AllFilesHaveImageFileExtensionAttributeTests
{
    private static AllFilesHaveImageFileExtensionAttribute BuildTestSubject() => new();

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

        Assert.Equal("AllFilesHaveImageFileExtensionAttribute only works with properties of type IEnumerable<IFormFile>.", exception.Message);
    }

    [Theory]
    [InlineData("test_image.png")]
    [InlineData("test_image.jpg")]
    [InlineData("test_image.jpeg")]
    [InlineData("test_image.gif")]
    [InlineData("test_image.bmp")]
    public void GetValidationResult_ReturnsSuccess_WhenFilesWithImageExtensionsAreGiven(string fileName)
    {
        // Arrange
        var attribute = BuildTestSubject();

        var file_1 = new Mock<IFormFile>();
        file_1.Setup(f => f.FileName).Returns(fileName);

        var file_2 = new Mock<IFormFile>();
        file_2.Setup(f => f.FileName).Returns(fileName);

        object files = new List<IFormFile>() { file_1.Object, file_2.Object };

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(files, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAFileWithoutAnImageExtensionIsGiven()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var file_1 = new Mock<IFormFile>();
        file_1.Setup(f => f.FileName).Returns("test_image.png");

        var file_2 = new Mock<IFormFile>();
        file_2.Setup(f => f.FileName).Returns("test_image.pdf");

        object files = new List<IFormFile>() { file_1.Object, file_2.Object };

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(files, context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Only the following file extensions are allowed: .png, .jpg, .jpeg, .gif, .bmp.", result?.ErrorMessage);
    }
}