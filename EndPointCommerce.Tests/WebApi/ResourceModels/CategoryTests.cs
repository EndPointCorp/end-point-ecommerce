namespace EndPointCommerce.Tests.WebApi.ResourceModels;

public class CategoryTests
{
    [Fact]
    public void FromEntity_ReturnsANewInstance_WithTheCorrectFields()
    {
        // Arrange
        var entity = new EndPointCommerce.Domain.Entities.Category
        {
            Id = 10,
            Name = "test_name",
            UrlKey = "test_url_key",

            MetaTitle = "test_meta_title",
            MetaKeywords = "test_meta_keywords",
            MetaDescription = "test_meta_description",

            IsEnabled = true
        };

        var result = EndPointCommerce.WebApi.ResourceModels.Category.FromEntity(entity, "test_image_url");

        // Act & Assert
        Assert.Equal(10, result.Id);
        Assert.Equal("test_name", result.Name);
        Assert.Equal("test_url_key", result.UrlKey);
        Assert.Equal("test_meta_title", result.Metadata!.Title);
        Assert.Equal("test_meta_keywords", result.Metadata.Keywords);
        Assert.Equal("test_meta_description", result.Metadata.Description);
    }

    [Fact]
    public void FromListOfEntities_ReturnsAListOfNewInstances()
    {
        // Arrange
        var entities = new List<EndPointCommerce.Domain.Entities.Category>
        {
            new()
            {
                Id = 10,
                Name = "test_name_1"
            },
            new()
            {
                Id = 20,
                Name = "test_name_2"
            }
        };

        var result = EndPointCommerce.WebApi.ResourceModels.Category.FromListOfEntities(entities, "test_image_url");

        // Act & Assert
        Assert.Equal(entities.Count, result.Count);
        Assert.Equal(10, result.First().Id);
        Assert.Equal("test_name_1", result.First().Name);
        Assert.Equal(20, result.Last().Id);
        Assert.Equal("test_name_2", result.Last().Name);
    }
}
