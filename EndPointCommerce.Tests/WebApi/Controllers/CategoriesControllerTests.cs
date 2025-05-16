using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Tests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointCommerce.Tests.WebApi.Controllers;

public class CategoriesControllerTests : IntegrationTests
{
    public CategoriesControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private Category CreateNewCategory(string name, bool isEnabled = true)
    {
        var newCategory = new Category()
        {
            Name = name,
            IsEnabled = isEnabled
        };

        dbContext.Categories.Add(newCategory);
        dbContext.SaveChanges();

        return newCategory;
    }

    [Fact]
    public async Task GetCategories_ReturnsEnabledCategories()
    {
        // Arrange
        CreateNewCategory("test_category_1");
        CreateNewCategory("test_category_2");
        CreateNewCategory("test_category_3", false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<EndPointCommerce.WebApi.ResourceModels.Category>>();

        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count);
        Assert.Contains(categories, c => c.Name == "test_category_1");
        Assert.Contains(categories, c => c.Name == "test_category_2");
        Assert.DoesNotContain(categories, c => c.Name == "test_category_3");
    }

    [Fact]
    public async Task GetCategories_ReturnsEmptyList_WhenNoEnabledCategoriesExist()
    {
        // Arrange
        CreateNewCategory("test_category_1", false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<EndPointCommerce.WebApi.ResourceModels.Category>>();

        Assert.NotNull(categories);
        Assert.Empty(categories);
    }
}