using EndPointCommerce.WebApi.Controllers;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.UnitTests.WebApi.Controllers;

public class ProductsControllerTests
{
    private static IList<Product> BuildProducts() => [
        new() { Name = "test_name_1", Sku = "test_sku_1", BasePrice = 10.00M },
        new() { Name = "test_name_2", Sku = "test_sku_2", BasePrice = 20.00M }
    ];

    private static Mock<IProductRepository> BuildMockRepository(
        IList<Product> products = null!,
        Product? product = null
    ) {
        var mockRepository = new Mock<IProductRepository>();
        mockRepository
            .Setup(m => m.FetchAllAsync(true))
            .ReturnsAsync(products);

        mockRepository
            .Setup(m => m.FetchAllByCategoryIdAsync(It.IsAny<int>(), true))
            .ReturnsAsync(products);

        mockRepository
            .Setup(m => m.FetchAllByCategoryUrlKeyAsync(It.IsAny<string>(), true))
            .ReturnsAsync(products);

        mockRepository
            .Setup(m => m.FindByIdAsync(10, true, false))
            .ReturnsAsync(product);

        mockRepository
            .Setup(m => m.FindByUrlKeyAsync("test_url_key", true))
            .ReturnsAsync(product);

        return mockRepository;
    }

    private static Mock<IConfiguration> BuildMockConfiguration()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration
            .Setup(m => m["ProductImagesUrlPath"])
            .Returns("mock_product_images_url_path");

        return mockConfiguration;
    }

    private static void AssertResultMatchesEntities(
        IList<Product> products,
        ActionResult<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Product>> result
    ) {
        Assert.Equal(products.Count, result.Value!.Count());
        Assert.Equal(products.Select(c => c.Id), result.Value!.Select(rm => rm.Id));
        Assert.Equal(products.Select(c => c.Name), result.Value!.Select(rm => rm.Name));
    }

    [Fact]
    public async Task GetProducts_ReturnsTheListOfProducts()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products: products);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProducts();

        // Assert
        AssertResultMatchesEntities(products, result);
    }

    [Fact]
    public async Task GetProducts_CallsOnTheRepositoryToFetchAllProducts()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products: products);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        await controller.GetProducts();

        // Assert
        mockRepository.Verify(m => m.FetchAllAsync(true), Times.Once());
    }

    [Fact]
    public async Task GetProductsByCategoryId_ReturnsTheListOfProducts()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products: products);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProductsByCategoryId(10);

        // Assert
        AssertResultMatchesEntities(products, result);
    }

    [Fact]
    public async Task GetProductsByCategoryId_CallsOnTheRepositoryToFetchProductsByCategory()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products: products);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        await controller.GetProductsByCategoryId(10);

        // Assert
        mockRepository.Verify(m => m.FetchAllByCategoryIdAsync(10, true), Times.Once());
    }

    [Fact]
    public async Task GetProductsByCategoryUrlKey_ReturnsTheListOfProducts()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products: products);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProductsByCategoryUrlKey("test_url_key");

        // Assert
        AssertResultMatchesEntities(products, result);
    }

    [Fact]
    public async Task GetProductsByCategoryUrlKey_CallsOnTheRepositoryToFetchProductsByCategory()
    {
        // Arrange
        var products = BuildProducts();
        var mockRepository = BuildMockRepository(products: products);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        await controller.GetProductsByCategoryUrlKey("test_url_key");

        // Assert
        mockRepository.Verify(m => m.FetchAllByCategoryUrlKeyAsync("test_url_key", true), Times.Once());
    }

    [Fact]
    public async Task GetProduct_CallsOnTheRepositoryToFindTheProductById()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        await controller.GetProduct(10);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, true, false), Times.Once());
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenTheProductIsNotFound()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProduct(10);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetProduct_ReturnsTheFoundProduct_WhenTheProductIsFound()
    {
        // Arrange
        var product = new Product() { Name = "test_name_1", Sku = "test_sku_1", BasePrice = 10.00M };

        var mockRepository = BuildMockRepository(product: product);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProduct(10);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal("test_name_1", result.Value.Name);
        Assert.Equal("test_sku_1", result.Value.Sku);
        Assert.Equal(10.00M, result.Value.BasePrice);
    }

    [Fact]
    public async Task GetProductByUrlKey_CallsOnTheRepositoryToFindTheProductByUrlKey()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        await controller.GetProductByUrlKey("test_url_key");

        // Assert
        mockRepository.Verify(m => m.FindByUrlKeyAsync("test_url_key", true), Times.Once());
    }

    [Fact]
    public async Task GetProductByUrlKey_ReturnsNotFound_WhenTheProductIsNotFound()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProductByUrlKey("test_url_key");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetProductByUrlKey_ReturnsTheFoundProduct_WhenTheProductIsFound()
    {
        // Arrange
        var product = new Product() { Name = "test_name_1", Sku = "test_sku_1", BasePrice = 10.00M };

        var mockRepository = BuildMockRepository(product: product);
        var mockConfiguration = BuildMockConfiguration();
        var controller = new ProductsController(mockRepository.Object, mockConfiguration.Object);

        // Act
        var result = await controller.GetProductByUrlKey("test_url_key");

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal("test_name_1", result.Value.Name);
        Assert.Equal("test_sku_1", result.Value.Sku);
        Assert.Equal(10.00M, result.Value.BasePrice);
    }
}
