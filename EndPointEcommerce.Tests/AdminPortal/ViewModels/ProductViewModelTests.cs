using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.AdminPortal.ViewModels;
using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointEcommerce.Tests.AdminPortal.ViewModels;

public class ProductViewModelTests
{
    private ProductViewModel BuildSubject() =>
        new() { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

    private (bool, List<ValidationResult>) RunValidation(ProductViewModel subject)
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
        var subject = BuildSubject();
        subject.MainImage = null;

        // Act && Assert
        Assert.Empty(subject.MainImageUrl);
    }

    [Fact]
    public void ThumbnailImageUrl_ReturnsEmpty_WhenThumbnailImageIsNull()
    {
        // Arrange
        var subject = BuildSubject();
        subject.ThumbnailImage = null;

        // Act && Assert
        Assert.Empty(subject.ThumbnailImageUrl);
    }

    [Fact]
    public void MainImageUrl_ReturnsTheCalculatedLocationOfTheMainImageFile_WhenMainImageIsNotNull()
    {
        // Arrange
        var subject = BuildSubject();
        subject.MainImage = new() { FileName = "test_main_image_file_name" };


        // Act && Assert
        Assert.Equal("~/product-images/test_main_image_file_name", subject.MainImageUrl);
    }

    [Fact]
    public void ThumbnailImageUrl_ReturnsTheCalculatedLocationOfTheThumbnailImageFile_WhenThumbnailImageIsNotNull()
    {
        // Arrange
        var subject = BuildSubject();
        subject.ThumbnailImage = new() { FileName = "test_thumbnail_image_file_name" };


        // Act && Assert
        Assert.Equal("~/product-images/test_thumbnail_image_file_name", subject.ThumbnailImageUrl);
    }

    [Fact]
    public void Validation_Succeeds_WhenThereIsNoMainImageFile()
    {
        // Arrange
        var subject = BuildSubject();
        subject.MainImageFile = null;

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validation_Succeeds_WhenThereIsNoThumbnailImageFile()
    {
        // Arrange
        var subject = BuildSubject();
        subject.ThumbnailImageFile = null;

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

        var subject = BuildSubject();
        subject.MainImageFile = mockFormFile.Object;

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
    public void Validation_Succeeds_WhenTheThumbnailImageFileIsANonZeroLengthFileWithAnAllowedExtension(string extension)
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        mockFormFile
            .Setup(m => m.FileName)
            .Returns($"test_thumbnail_image_file_name{extension}");

        var subject = BuildSubject();
        subject.ThumbnailImageFile = mockFormFile.Object;

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

        var subject = BuildSubject();
        subject.MainImageFile = mockFormFile.Object;

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Contains("Only the following file extensions are allowed", results.First().ErrorMessage);
    }

    [Theory]
    [InlineData(".tiff")]
    [InlineData(".pdf")]
    [InlineData(".exe")]
    [InlineData("")]
    public void Validation_Fails_WhenTheThumbnailImageFileIsANonZeroLengthFileWithADisallowedExtension(string extension)
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        mockFormFile
            .Setup(m => m.FileName)
            .Returns($"test_thumbnail_image_file_name{extension}");

        var subject = BuildSubject();
        subject.ThumbnailImageFile = mockFormFile.Object;

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

        var subject = BuildSubject();
        subject.MainImageFile = mockFormFile.Object;

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Equal("The selected file appears to be empty.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validation_Fails_WhenTheThumbnailImageFileIsAZeroLengthFile()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile
            .Setup(m => m.Length)
            .Returns(0);

        mockFormFile
            .Setup(m => m.FileName)
            .Returns("test_thumbnail_image_file_name.png");

        var subject = BuildSubject();
        subject.ThumbnailImageFile = mockFormFile.Object;

        // Act
        var (isValid, results) = RunValidation(subject);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Equal("The selected file appears to be empty.", results.First().ErrorMessage);
    }

    [Fact]
    public void FromModel_CreatesANewObject_WithDataFromTheGivenEntity()
    {
        // Arrange
        var entity = new Product
        {
            Id = 10,

            Name = "test_name",
            Sku = "test_sku",
            CategoryId = 100,

            IsEnabled = true,
            IsInStock = false,
            IsDiscounted = true,

            BasePrice = 50.00M,
            DiscountAmount = 15.00M,

            Description = "test_description",
            ShortDescription = "test_short_description",
            Weight = 5.00M,

            UrlKey = "test-url-key",
            MetaTitle = "test_meta_title",
            MetaKeywords = "test, meta, keywords",
            MetaDescription = "test_meta_description"
        };

        // Act
        var result = ProductViewModel.FromModel(entity);

        // Assert
        Assert.Equal(10, result.Id);

        Assert.Equal("test_name", result.Name);
        Assert.Equal("test_sku", result.Sku);
        Assert.Equal(100, result.CategoryId);

        Assert.True(result.IsEnabled);
        Assert.False(result.IsInStock);
        Assert.True(result.IsDiscounted);

        Assert.Equal(50.00M, result.BasePrice);
        Assert.Equal(15.00M, result.DiscountAmount);

        Assert.Equal("test_description", result.Description);
        Assert.Equal("test_short_description", result.ShortDescription);
        Assert.Equal(5.00M, result.Weight);

        Assert.Equal("test-url-key", result.UrlKey);
        Assert.Equal("test_meta_title", result.MetaTitle);
        Assert.Equal("test, meta, keywords", result.MetaKeywords);
        Assert.Equal("test_meta_description", result.MetaDescription);
    }

    [Fact]
    public void ToInputPayload_ShouldReturnANewProductInputPayloadBasedOnThisObject()
    {
        // Arrange
        var mainImageFileMock = new Mock<IFormFile>();
        var thumbnailImageFileMock = new Mock<IFormFile>();
        var additionalImageFilesMock = new List<IFormFile> { new Mock<IFormFile>().Object };

        var productViewModel = new ProductViewModel
        {
            Id = 1,
            Name = "test_name",
            Sku = "test_sku",
            CategoryId = 2,
            IsEnabled = true,
            IsInStock = true,
            IsDiscounted = false,
            BasePrice = 100,
            DiscountAmount = 10,
            Description = "test_description",
            ShortDescription = "test_short_description",
            Weight = 1.5m,
            UrlKey = "test_url_key",
            MetaTitle = "test_meta_title",
            MetaKeywords = "test_meta_keywords",
            MetaDescription = "test_meta_description",
            MainImageFile = mainImageFileMock.Object,
            ThumbnailImageFile = thumbnailImageFileMock.Object,
            AdditionalImageFiles = additionalImageFilesMock
        };

        // Act
        var inputPayload = productViewModel.ToInputPayload();

        // Assert
        Assert.Equal(productViewModel.Id, inputPayload.Id);
        Assert.Equal(productViewModel.Name, inputPayload.Name);
        Assert.Equal(productViewModel.Sku, inputPayload.Sku);
        Assert.Equal(productViewModel.CategoryId, inputPayload.CategoryId);
        Assert.Equal(productViewModel.IsEnabled, inputPayload.IsEnabled);
        Assert.Equal(productViewModel.IsInStock, inputPayload.IsInStock);
        Assert.Equal(productViewModel.IsDiscounted, inputPayload.IsDiscounted);
        Assert.Equal(productViewModel.BasePrice, inputPayload.BasePrice);
        Assert.Equal(productViewModel.DiscountAmount, inputPayload.DiscountAmount);
        Assert.Equal(productViewModel.Description, inputPayload.Description);
        Assert.Equal(productViewModel.ShortDescription, inputPayload.ShortDescription);
        Assert.Equal(productViewModel.Weight, inputPayload.Weight);
        Assert.Equal(productViewModel.UrlKey, inputPayload.UrlKey);
        Assert.Equal(productViewModel.MetaTitle, inputPayload.MetaTitle);
        Assert.Equal(productViewModel.MetaKeywords, inputPayload.MetaKeywords);
        Assert.Equal(productViewModel.MetaDescription, inputPayload.MetaDescription);
        Assert.Equal(productViewModel.MainImageFile, inputPayload.MainImageFile);
        Assert.Equal(productViewModel.ThumbnailImageFile, inputPayload.ThumbnailImageFile);
        Assert.Equal(productViewModel.AdditionalImageFiles, inputPayload.AdditionalImageFiles);
    }
}
