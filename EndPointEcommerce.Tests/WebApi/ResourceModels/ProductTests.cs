namespace EndPointEcommerce.Tests.WebApi.ResourceModels;

public class ProductTests
{
    private EndPointEcommerce.Domain.Entities.Product CreateProduct()
    {
        return new EndPointEcommerce.Domain.Entities.Product
        {
            Id = 10,
            Name = "test_name",

            UrlKey = "test_url_key",
            MetaTitle = "test_meta_title",
            MetaKeywords = "test_meta_keywords",
            MetaDescription = "test_meta_description",

            IsEnabled = true,
            IsInStock = false,

            Sku = "test_sku",
            BasePrice = 50.00M,

            Description = "test_description",
            ShortDescription = "test_short_description",
            Weight = 5.00M
        };
    }

    [Fact]
    public void FromEntity_ReturnsANewInstance_WithTheCorrectFields()
    {

        // Arrange
        var entity = CreateProduct();
        entity.CategoryId = 20;
        entity.Category = new EndPointEcommerce.Domain.Entities.Category {
            Id = 20,
            Name = "test_category_name",
            UrlKey = "test_category_url_key",
        };

        var result = EndPointEcommerce.WebApi.ResourceModels.Product.FromEntity(entity, "test_image_url");

        // Act & Assert
        Assert.Equal(10, result.Id);
        Assert.Equal("test_name", result.Name);
        Assert.Equal("test_url_key", result.UrlKey);
        Assert.Equal("test_meta_title", result.Metadata!.Title);
        Assert.Equal("test_meta_keywords", result.Metadata.Keywords);
        Assert.Equal("test_meta_description", result.Metadata.Description);
        Assert.False(result.IsInStock);

        Assert.Equal("test_sku", result.Sku);
        Assert.Equal(50.00M, result.BasePrice);

        Assert.Equal("test_description", result.Description);
        Assert.Equal("test_short_description", result.ShortDescription);
        Assert.Equal(5.00M, result.Weight);

        Assert.Equal(20, result.CategoryId);
        Assert.Equal("test_category_name", result.Category!.Name);
        Assert.Equal("test_category_url_key", result.Category.UrlKey);
    }

    [Fact]
    public void FromEntity_ReturnsANewInstanceWithNoCategory_WhenTheGivenProductEntityHasNoCategory()
    {
        // Arrange
        var entity = CreateProduct();
        entity.CategoryId = null;
        entity.Category = null;

        var result = EndPointEcommerce.WebApi.ResourceModels.Product.FromEntity(entity, "test_image_url");

        // Act & Assert
        Assert.Null(result.CategoryId);
        Assert.Null(result.Category);
    }

    [Fact]
    public void FromListOfEntities_ReturnsAListOfNewInstances()
    {
        // Arrange
        var entities = new List<EndPointEcommerce.Domain.Entities.Product>
        {
            new()
            {
                Id = 10,
                Name = "test_name_1",
                Sku = "test_sku_1",
                BasePrice = 10.00M
            },
            new()
            {
                Id = 20,
                Name = "test_name_2",
                Sku = "test_sku_2",
                BasePrice = 20.00M
            }
        };

        var result = EndPointEcommerce.WebApi.ResourceModels.Product.FromListOfEntities(entities, "test_image_url");

        // Act & Assert
        Assert.Equal(entities.Count, result.Count);

        Assert.Equal(10, result.First().Id);
        Assert.Equal("test_name_1", result.First().Name);
        Assert.Equal("test_sku_1", result.First().Sku);
        Assert.Equal(10.00M, result.First().BasePrice);

        Assert.Equal(20, result.Last().Id);
        Assert.Equal("test_name_2", result.Last().Name);
        Assert.Equal("test_sku_2", result.Last().Sku);
        Assert.Equal(20.00M, result.Last().BasePrice);
    }
}
