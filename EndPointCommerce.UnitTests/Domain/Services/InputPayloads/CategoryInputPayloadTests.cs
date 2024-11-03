using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointCommerce.UnitTests.Domain.Services.InputPayloads;

public class CategoryInputPayloadTests
{
    [Fact]
    public void UploadedMainImageFile_ReturnsTrue_WhenMainImageFileIsSetToANonZeroLengthFileReference()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var subject = new CategoryInputPayload { Name = "test_name", MainImageFile = mockFormFile.Object };

        // Act && Assert
        Assert.True(subject.UploadedMainImageFile);
    }

    [Fact]
    public void UploadedMainImageFile_ReturnsFalse_WhenMainImageFileIsNull()
    {
        // Arrange
        var subject = new CategoryInputPayload { Name = "test_name", MainImageFile = null };

        // Act && Assert
        Assert.False(subject.UploadedMainImageFile);
    }

    [Fact]
    public void UploadedMainImageFile_ReturnsFalse_WhenMainImageFileIsZeroLength()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(0);

        var subject = new CategoryInputPayload { Name = "test_name", MainImageFile = mockFormFile.Object };

        // Act && Assert
        Assert.False(subject.UploadedMainImageFile);
    }

    [Fact]
    public void CopyInto_PopulatesTheGivenEntity_UsingOnlyThePermittedParams()
    {
        // Arrange
        var entity = new Category { Name = "test_name" };

        var subject = new CategoryInputPayload
        {
            Name = "test_name",
            IsEnabled = true,
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
        Assert.True(entity.IsEnabled);
        Assert.Equal("test-url-key", entity.UrlKey);
        Assert.Equal("test_meta_title", entity.MetaTitle);
        Assert.Equal("test, meta, keywords", entity.MetaKeywords);
        Assert.Equal("test_meta_description", entity.MetaDescription);

        Assert.Equal(0, entity.Id);
        Assert.Null(entity.DateCreated);
        Assert.Null(entity.DateModified);
    }
}