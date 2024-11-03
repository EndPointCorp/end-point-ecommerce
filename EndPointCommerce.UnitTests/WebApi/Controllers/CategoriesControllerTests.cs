using EndPointCommerce.WebApi.Controllers;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Moq;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.UnitTests.WebApi.Controllers;

public class CategoriesControllerTests
{
    private static IList<Category> BuildCategories() => [
        new() { Name = "test_name_1" },
        new() { Name = "test_name_2" }
    ];

    private static Mock<ICategoryRepository> BuildMockRepository(IList<Category> categories)
    {
        var mockRepository = new Mock<ICategoryRepository>();
        mockRepository
            .Setup(m => m.FetchAllAsync(true))
            .ReturnsAsync(categories);

        return mockRepository;
    }

    private static Mock<IConfiguration> BuildMockConfiguration()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration
            .Setup(m => m["CategoryImagesUrlPath"])
            .Returns("mock_category_images_url_path");

        return mockConfiguration;
    }

    [Fact]
    public async Task GetCategories_ReturnsTheListOfCategories()
    {
        // Arrange
        var categories = BuildCategories();
        var mockRepository = BuildMockRepository(categories);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new CategoriesController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetCategories();

        // Assert
        Assert.Equal(categories.Count, result.Value!.Count());
        Assert.Equal(categories.Select(c => c.Id), result.Value!.Select(rm => rm.Id));
        Assert.Equal(categories.Select(c => c.Name), result.Value!.Select(rm => rm.Name));
    }

    [Fact]
    public async Task GetCategories_CallsOnTheRepositoryToFetchAllCategories()
    {
        // Arrange
        var categories = BuildCategories();
        var mockRepository = BuildMockRepository(categories);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new CategoriesController(mockRepository.Object, mockConfiguration.Object);

        // Act
        await controller.GetCategories();

        // Assert
        mockRepository.Verify(m => m.FetchAllAsync(true), Times.Once());
    }
}
