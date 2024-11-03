using EndPointCommerce.AdminPortal.Pages.Categories;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Moq;

namespace EndPointCommerce.UnitTests.AdminPortal.Pages.Categories;

public class IndexPageModelTests
{
    private static IList<Category> BuildCategories() => [
        new() { Name = "test_name_1" },
        new() { Name = "test_name_2" }
    ];

    private static Mock<ICategoryRepository> BuildMockRepository(IList<Category> categories)
    {
        var mockRepository = new Mock<ICategoryRepository>();
        mockRepository
            .Setup(m => m.FetchAllAsync(false))
            .ReturnsAsync(categories);

        return mockRepository;
    }

    [Fact]
    public async Task OnGetAsync_PopulatesTheListOfCategories()
    {
        // Arrange
        var categories = BuildCategories();
        var mockRepository = BuildMockRepository(categories);
        var pageModel = new IndexModel(mockRepository.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.Equal(categories, pageModel.Categories);
    }

    [Fact]
    public async Task OnGetAsync_CallsOnTheRepositoryToFetchAllCategories()
    {
        // Arrange
        var categories = BuildCategories();
        var mockRepository = BuildMockRepository(categories);
        var pageModel = new IndexModel(mockRepository.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        mockRepository.Verify(m => m.FetchAllAsync(false), Times.Once());
    }
}
