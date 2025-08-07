using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Tests.Domain.Entities;

public class ProductTests : BaseSeoEntitiesTests
{
    protected override BaseSeoEntity BuildSubjectWithUrlKey(string urlKey) =>
        new Product { Name = "test_name", Sku = "test_sku", UrlKey = urlKey, BasePrice = 10.00M };

    [Fact]
    public void Equals_ReturnsTrue_WhenTheObjectsBeingComparedHaveTheSameId()
    {
        // Arrange
        var thisOne = new Product { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var thatOne = new Product { Id = 10, Name = "test_name_changed", Sku = "test_sku_changed", BasePrice = 20.00M };

        // Act & Assert
        Assert.True(thisOne.Equals(thatOne));
    }

    [Fact]
    public void HasMainImage_ShouldReturnTrue_WhenMainImageIsNotNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 15.00M,
            MainImage = new ProductImage { FileName = "test_file_name" }
        };

        // Assert
        Assert.True(product.HasMainImage);
    }

    [Fact]
    public void HasMainImage_ShouldReturnFalse_WhenMainImageIsNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 15.00M
        };

        // Assert
        Assert.False(product.HasMainImage);
    }

    [Fact]
    public void HasThumbnailImage_ShouldReturnTrue_WhenMainImageIsNotNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 15.00M,
            ThumbnailImage = new ProductImage { FileName = "test_file_name" }
        };

        // Assert
        Assert.True(product.HasThumbnailImage);
    }

    [Fact]
    public void HasThumbnailImage_ShouldReturnFalse_WhenMainImageIsNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 15.00M
        };

        // Assert
        Assert.False(product.HasThumbnailImage);
    }

    [Fact]
    public void GetAdditionalImageById_ReturnsCorrectImage_WhenImageExists()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 15.00M,

            AdditionalImages =
            [
                new ProductImage { Id = 1, FileName = "test_file_name_1" },
                new ProductImage { Id = 2, FileName = "test_file_name_2" }
            ]
        };

        // Act
        var result = product.GetAdditionalImageById(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
    }

    [Fact]
    public void GetAdditionalImageById_ReturnsNull_WhenImageDoesNotExist()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 15.00M,

            AdditionalImages =
            [
                new ProductImage { Id = 1, FileName = "test_file_name_1" },
                new ProductImage { Id = 2, FileName = "test_file_name_2" }
            ]
        };

        // Act
        var result = product.GetAdditionalImageById(3);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DiscountedPrice_ReturnsTheBasePriceMinusTheDiscountAmount()
    {
        // Arrange
        var subject = new Product { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M, DiscountAmount = 5.15M };

        // Act & Assert
        Assert.Equal(4.85M, subject.DiscountedPrice);
    }

    [Fact]
    public void GetActualPrice_ShouldReturnBasePrice_WhenNotDiscounted()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 100M,
            IsDiscounted = false
        };

        // Act
        var actualPrice = product.GetActualPrice();

        // Assert
        Assert.Equal(100M, actualPrice);
    }

    [Fact]
    public void GetActualPrice_ShouldReturnDiscountedPrice_WhenDiscounted()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 100M,
            IsDiscounted = true,
            DiscountAmount = 20M
        };

        // Act
        var actualPrice = product.GetActualPrice();

        // Assert
        Assert.Equal(80M, actualPrice);
    }

    [Fact]
    public void GetActualPrice_ShouldReturnBasePrice_WhenDiscountAmountIsNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 100M,
            IsDiscounted = true,
            DiscountAmount = null
        };

        // Act
        var actualPrice = product.GetActualPrice();

        // Assert
        Assert.Equal(100M, actualPrice);
    }

    [Fact]
    public void GetActualPrice_ShouldReturnZero_WhenDiscountIsHigherThanBasePrice()
    {
        // Arrange
        var product = new Product
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 50M,
            IsDiscounted = true,
            DiscountAmount = 60M
        };

        // Act
        var actualPrice = product.GetActualPrice();

        // Assert
        Assert.Equal(0M, actualPrice);
    }
}
