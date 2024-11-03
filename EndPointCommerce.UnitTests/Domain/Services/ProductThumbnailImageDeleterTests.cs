using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EndPointCommerce.UnitTests.Domain.Services;

public class ProductThumbnailImageDeleterTests
{
    private static Mock<IProductRepository> BuildMockRepository(Product? product = null)
    {
        var mockRepository = new Mock<IProductRepository>();

        mockRepository
            .Setup(m => m.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(product);

        mockRepository
            .Setup(m => m.DeleteThumbnailImage(It.IsAny<Product>()));

        return mockRepository;
    }

    private static Mock<IFileService> BuildMockFileService()
    {
        var mockFileService = new Mock<IFileService>();

        mockFileService
            .Setup(m => m.DeleteFile(It.IsAny<string>()));

        return mockFileService;
    }

    private static Mock<IConfiguration> BuildMockConfiguration()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration
            .Setup(m => m["ProductImagesPath"])
            .Returns("mock_product_images_path");

        return mockConfiguration;
    }

    private static ProductThumbnailImageDeleter BuildSubject(
        Mock<IProductRepository>? repository = null,
        Mock<IFileService>? fileService = null,
        Mock<IConfiguration>? configuration = null
    ) {
        var mockRepository = repository ?? BuildMockRepository();
        var mockFileService = fileService ?? BuildMockFileService();
        var mockConfiguration = configuration ?? BuildMockConfiguration();

        return new ProductThumbnailImageDeleter(
            mockRepository.Object,
            mockFileService.Object,
            mockConfiguration.Object
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToFindAProduct()
    {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product);
        var service = BuildSubject(repository: mockRepository);

        // Act
        await service.Run(10);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, false, false), Times.Once());
    }

    [Fact]
    public async Task Run_ReturnsAnObjectRepresentingTheNewlyUpdatedProduct()
    {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product);
        var service = BuildSubject(repository: mockRepository);

        // Act
        var result = await service.Run(10);

        // Assert
        Assert.Equal("test_name", result.Name);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Run_ThrowsAnEntityNotFoundException_WhenTheProductCannotBeFound()
    {
        // Arrange
        var service = BuildSubject();

        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Run(10));
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToDeleteAFile_WhenTheProductToUpdateHasAThumbnailImage()
    {
        // Arrange
        var product = new Product()
        {
            Id = 10,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M,
            ThumbnailImage = new() { FileName = "test_thumbnail_image_file_name" }
        };

        var mockRepository = BuildMockRepository(product);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        // Act
        await service.Run(10);

        // Assert
        mockFileService.Verify(
            m => m.DeleteFile("mock_product_images_path/test_thumbnail_image_file_name"),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToDeleteTheProductThumbnailImageRecord_WhenTheProductToUpdateHasAThumbnailImage()
    {
        // Arrange
        var product = new Product()
        {
            Id = 10,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M,
            ThumbnailImage = new() { FileName = "test_thumbnail_image_file_name" }
        };

        var mockRepository = BuildMockRepository(product);
        var service = BuildSubject(repository: mockRepository);

        // Act
        await service.Run(10);

        // Assert
        mockRepository.Verify(m => m.DeleteThumbnailImage(product), Times.Once());
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheFileServiceToDeleteAFile_WhenTheProductToUpdateDoesNotHaveAThumbnailImage()
    {
        // Arrange
        var product = new Product()
        {
            Id = 10,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M,
            ThumbnailImage = null
        };

        var mockRepository = BuildMockRepository(product);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        // Act
        await service.Run(10);

        // Assert
        mockFileService.Verify(m => m.DeleteFile(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheRepositoryToDeleteTheProductThumbnailImageRecord_WhenTheProductToUpdateDoesNotHaveAThumbnailImage()
    {
        // Arrange
        var product = new Product()
        {
            Id = 10,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M,
            ThumbnailImage = null
        };

        var mockRepository = BuildMockRepository(product);
        var service = BuildSubject(repository: mockRepository);

        // Act
        await service.Run(10);

        // Assert
        mockRepository.Verify(m => m.DeleteThumbnailImage(It.IsAny<Product>()), Times.Never());
    }
}