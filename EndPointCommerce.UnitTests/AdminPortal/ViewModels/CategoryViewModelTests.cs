using System.ComponentModel.DataAnnotations;
using EndPointCommerce.AdminPortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointCommerce.UnitTests.AdminPortal.ViewModels;

public class CategoryViewModelTests
{
    private (bool, List<ValidationResult>) RunValidation(CategoryViewModel subject)
    {
        var context = new ValidationContext(subject);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(subject, context, results, true);

        return (isValid, results);
    }

    [Fact]
    public void MainImageUrl_ReturnsEmpty_WhenMainImageIsNull()
    {
        // Arrange
        var subject = new CategoryViewModel { Name = "test_name", MainImage = null };

        // Act && Assert
        Assert.Empty(subject.MainImageUrl);
    }

    [Fact]
    public void MainImageUrl_ReturnsTheCalculatedLocationOfTheMainImageFile_WhenMainImageIsNotNull()
    {
        // Arrange
        var subject = new CategoryViewModel
        {
            Name = "test_name",
            MainImage = new() { FileName = "test_main_image_file_name" }
        };

        // Act && Assert
        Assert.Equal("~/category-images/test_main_image_file_name", subject.MainImageUrl);
    }

    [Fact]
    public void Validation_Succeeds_WhenThereIsNoMainImageFile()
    {
        // Arrange
        var subject = new CategoryViewModel { Name = "test_name", MainImageFile = null };

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(".png")]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".gif")]
    [InlineData(".bmp")]
    public void Validation_Succeeds_WhenTheMainImageFileIsANonZeroLengthFileWithAnAllowedExtension(string extension)
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        mockFormFile
            .Setup(m => m.FileName)
            .Returns($"test_main_image_file_name{extension}");

        var subject = new CategoryViewModel { Name = "test_name", MainImageFile = mockFormFile.Object };

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(".tiff")]
    [InlineData(".pdf")]
    [InlineData(".exe")]
    [InlineData("")]
    public void Validation_Fails_WhenTheMainImageFileIsANonZeroLengthFileWithADisallowedExtension(string extension)
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        mockFormFile
            .Setup(m => m.FileName)
            .Returns($"test_main_image_file_name{extension}");

        var subject = new CategoryViewModel { Name = "test_name", MainImageFile = mockFormFile.Object };

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Contains("Only the following file extensions are allowed", results.First().ErrorMessage);
    }

    [Fact]
    public void Validation_Fails_WhenTheMainImageFileIsAZeroLengthFile()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile
            .Setup(m => m.Length)
            .Returns(0);

        mockFormFile
            .Setup(m => m.FileName)
            .Returns("test_main_image_file_name.png");

        var subject = new CategoryViewModel { Name = "test_name", MainImageFile = mockFormFile.Object };

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Equal("The selected file appears to be empty.", results.First().ErrorMessage);
    }

    [Fact]
    public void ToInputPayload_ShouldReturnANewCategoryInputPayloadBasedOnThisObject()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();

        var viewModel = new CategoryViewModel
        {
            Id = 1,
            Name = "test_name",
            IsEnabled = true,
            UrlKey = "test_url_key",
            MetaTitle = "test_meta_title",
            MetaKeywords = "test_meta_keywords",
            MetaDescription = "test_meta_description",
            MainImageFile = formFileMock.Object
        };

        // Act
        var inputPayload = viewModel.ToInputPayload();

        // Assert
        Assert.Equal(viewModel.Id, inputPayload.Id);
        Assert.Equal(viewModel.Name, inputPayload.Name);
        Assert.Equal(viewModel.IsEnabled, inputPayload.IsEnabled);
        Assert.Equal(viewModel.UrlKey, inputPayload.UrlKey);
        Assert.Equal(viewModel.MetaTitle, inputPayload.MetaTitle);
        Assert.Equal(viewModel.MetaKeywords, inputPayload.MetaKeywords);
        Assert.Equal(viewModel.MetaDescription, inputPayload.MetaDescription);
        Assert.Equal(viewModel.MainImageFile, inputPayload.MainImageFile);
    }
}
