using EndPointCommerce.AdminPortal.Pages.Products;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Moq;

namespace EndPointCommerce.Tests.AdminPortal.Pages.Products;

public class IndexPageModelTests
{
    private static IList<Product> BuildProducts() => [
        new() { Name = "test_name_1", Sku = "test_sku_1", BasePrice = 10.00M },
        new() { Name = "test_name_2", Sku = "test_sku_2", BasePrice = 20.00M }
    ];

    private static Mock<IProductRepository> BuildMockRepository(IList<Product> products)
    {
        var mockRepository = new Mock<IProductRepository>();
        mockRepository
            .Setup(m => m.FetchAllAsync(false))
            .ReturnsAsync(products);

        return mockRepository;
    }

    [Fact]
    public async Task OnGetAsync_PopulatesTheListOfProducts()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products);
        var pageModel = new IndexModel(mockRepository.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.Equal(products, pageModel.Products);
    }

    [Fact]
    public async Task OnGetAsync_CallsOnTheRepositoryToFetchAllProducts()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products);
        var pageModel = new IndexModel(mockRepository.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        mockRepository.Verify(m => m.FetchAllAsync(false), Times.Once());
    }
}
