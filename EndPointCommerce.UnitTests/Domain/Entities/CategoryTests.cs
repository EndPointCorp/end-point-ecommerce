using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.UnitTests.Domain.Entities;

public class CategoryTests : BaseSeoEntitiesTests
{
    protected override BaseSeoEntity BuildSubjectWithUrlKey(string urlKey) =>
        new Category { Name = "test_name", UrlKey = urlKey };

    [Fact]
    public void Equals_ReturnsTrue_WhenTheObjectsBeingComparedHaveTheSameId()
    {
        // Arrange
        var thisOne = new Category { Id = 10, Name = "test_name" };
        var thatOne = new Category { Id = 10, Name = "test_name_changed" };

        // Act & Assert
        Assert.True(thisOne.Equals(thatOne));
    }

    [Fact]
    public void HasMainImage_ShouldReturnTrue_WhenMainImageIsNotNull()
    {
        // Arrange
        var category = new Category
        {
            Name = "test_name",
            MainImage = new CategoryImage { FileName = "test_file_name" }
        };

        // Assert
        Assert.True(category.HasMainImage);
    }

    [Fact]
    public void HasMainImage_ShouldReturnFalse_WhenMainImageIsNull()
    {
        // Arrange
        var category = new Category
        {
            Name = "test_name"
        };

        // Assert
        Assert.False(category.HasMainImage);
    }
}
