using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Services;

namespace EndPointCommerce.Tests.WebApi.Services;

public class ImageUrlBuilderTests
{
    [Fact]
    public void GetImageUrl_ShouldReturnNull_WhenImageIsNull()
    {
        // Arrange
        Image? image = null;
        var imagesUrlPath = "http://example.com/images";

        // Act
        var result = ImageUrlBuilder.GetImageUrl(image, imagesUrlPath);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetImageUrl_ShouldReturnNull_WhenImagesUrlPathIsNull()
    {
        // Arrange
        var image = new Image { FileName = "image.jpg" };
        string? imagesUrlPath = null;

        // Act
        var result = ImageUrlBuilder.GetImageUrl(image, imagesUrlPath);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetImageUrl_ShouldReturnCorrectUrl()
    {
        // Arrange
        var image = new Image { FileName = "image.jpg" };
        var imagesUrlPath = "http://example.com/images";

        // Act
        var result = ImageUrlBuilder.GetImageUrl(image, imagesUrlPath);

        // Assert
        Assert.Equal("http://example.com/images/image.jpg", result);
    }
}