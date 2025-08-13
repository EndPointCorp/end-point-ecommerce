using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Services.InputPayloads;

public class ProductInputPayloadTests
{
    private ProductInputPayload BuildSubject() =>
        new() { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

    [Fact]
    public void UploadedMainImageFile_ReturnsTrue_WhenMainImageFileIsSetToANonZeroLengthFileReference()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var subject = BuildSubject();
        subject.MainImageFile = mockFormFile.Object;

        // Act && Assert
        Assert.True(subject.UploadedMainImageFile);
    }

    [Fact]
    public void UploadedThumbnailImageFile_ReturnsTrue_WhenThumbnailImageFileIsSetToANonZeroLengthFileReference()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var subject = BuildSubject();
        subject.ThumbnailImageFile = mockFormFile.Object;

        // Act && Assert
        Assert.True(subject.UploadedThumbnailImageFile);
    }

    [Fact]
    public void UploadedMainImageFile_ReturnsFalse_WhenMainImageFileIsNull()
    {
        // Arrange
        var subject = BuildSubject();
        subject.MainImageFile = null;


        // Act && Assert
        Assert.False(subject.UploadedMainImageFile);
    }

    [Fact]
    public void UploadedThumbnailImageFile_ReturnsFalse_WhenThumbnailImageFileIsNull()
    {
        // Arrange
        var subject = BuildSubject();
        subject.ThumbnailImageFile = null;


        // Act && Assert
        Assert.False(subject.UploadedThumbnailImageFile);
    }

    [Fact]
    public void UploadedMainImageFile_ReturnsFalse_WhenMainImageFileIsZeroLength()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(0);

        var subject = BuildSubject();
        subject.MainImageFile = mockFormFile.Object;


        // Act && Assert
        Assert.False(subject.UploadedMainImageFile);
    }

    [Fact]
    public void UploadedThumbnailImageFile_ReturnsFalse_WhenThumbnailImageFileIsZeroLength()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(0);

        var subject = BuildSubject();
        subject.ThumbnailImageFile = mockFormFile.Object;


        // Act && Assert
        Assert.False(subject.UploadedThumbnailImageFile);
    }

    [Fact]
    public void CopyInto_PopulatesTheGivenEntity_UsingOnlyThePermittedParams()
    {
        // Arrange
        var entity = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 50.00M
        };

        var subject = new ProductInputPayload
        {
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
            MetaDescription = "test_meta_description",

            Id = 10,
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };

        // Act
        subject.CopyInto(entity);

        // Assert
        Assert.Equal("test_name", entity.Name);
        Assert.Equal("test_sku", entity.Sku);
        Assert.Equal(100, entity.CategoryId);

        Assert.True(entity.IsEnabled);
        Assert.False(entity.IsInStock);
        Assert.True(entity.IsDiscounted);

        Assert.Equal(50.00M, entity.BasePrice);
        Assert.Equal(15.00M, entity.DiscountAmount);

        Assert.Equal("test_description", entity.Description);
        Assert.Equal("test_short_description", entity.ShortDescription);
        Assert.Equal(5.00M, entity.Weight);

        Assert.Equal("test-url-key", entity.UrlKey);
        Assert.Equal("test_meta_title", entity.MetaTitle);
        Assert.Equal("test, meta, keywords", entity.MetaKeywords);
        Assert.Equal("test_meta_description", entity.MetaDescription);

        Assert.Equal(0, entity.Id);
        Assert.Null(entity.DateCreated);
        Assert.Null(entity.DateModified);
    }
}
