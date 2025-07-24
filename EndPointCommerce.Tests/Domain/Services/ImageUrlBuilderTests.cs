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
        var imagesUrl = "http://example.com/images";

        // Act
        var result = ImageUrlBuilder.GetImageUrl(image, imagesUrl);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetImageUrl_ShouldReturnNull_WhenImagesUrlIsNull()
    {
        // Arrange
        var image = new Image { FileName = "image.jpg" };
        string? imagesUrl = null;

        // Act
        var result = ImageUrlBuilder.GetImageUrl(image, imagesUrl);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetImageUrl_ShouldReturnCorrectUrl()
    {
        // Arrange
        var image = new Image { FileName = "image.jpg" };
        var imagesUrl = "http://example.com/images";

        // Act
        var result = ImageUrlBuilder.GetImageUrl(image, imagesUrl);

        // Assert
        Assert.Equal("http://example.com/images/image.jpg", result);
    }
}