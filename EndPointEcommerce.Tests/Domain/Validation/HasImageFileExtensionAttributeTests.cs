// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using EndPointEcommerce.Domain.Validation;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Validation;

public class HasImageFileExtensionAttributeTests
{
    private static HasImageFileExtensionAttribute BuildTestSubject() => new();

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

        Assert.Equal("HasImageFileExtensionAttribute only works with properties of type IFormFile.", exception.Message);
    }

    [Theory]
    [InlineData("test_image.png")]
    [InlineData("test_image.jpg")]
    [InlineData("test_image.jpeg")]
    [InlineData("test_image.gif")]
    [InlineData("test_image.bmp")]
    public void GetValidationResult_ReturnsSuccess_WhenAFileWithAnImageExtensionIsGiven(string FileName)
    {
        // Arrange
        var attribute = BuildTestSubject();
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns(FileName);

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(file.Object, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAFileWithoutAnImageExtensionIsGiven()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns("test_image.pdf");

        var context = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(file.Object, context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Only the following file extensions are allowed: .png, .jpg, .jpeg, .gif, .bmp.", result?.ErrorMessage);
    }
}