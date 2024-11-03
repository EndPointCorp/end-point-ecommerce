using System.Linq.Expressions;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EndPointCommerce.UnitTests.Domain.Services;

public class ProductUpdaterTests
{
    private static Mock<IProductRepository> BuildMockRepository(Product? product = null, bool exists = true)
    {
        var mockRepository = new Mock<IProductRepository>();

        mockRepository
            .Setup(m => m.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(product);

        mockRepository
            .Setup(m => m.ExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(exists);

        mockRepository
            .Setup(m => m.UpdateAsync(It.IsAny<Product>()));

        mockRepository
            .Setup(m => m.DeleteMainImage(It.IsAny<Product>()));

        mockRepository
            .Setup(m => m.DeleteThumbnailImage(It.IsAny<Product>()));

        return mockRepository;
    }

    private static Mock<IFileService> BuildMockFileService()
    {
        var mockFileService = new Mock<IFileService>();

        mockFileService
            .Setup(m => m.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync("test_file_name");

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

    private static ProductUpdater BuildSubject(
        Mock<IProductRepository>? repository = null,
        Mock<IFileService>? fileService = null,
        Mock<IConfiguration>? configuration = null
    ) {
        var mockRepository = repository ?? BuildMockRepository();
        var mockFileService = fileService ?? BuildMockFileService();
        var mockConfiguration = configuration ?? BuildMockConfiguration();

        return new ProductUpdater(
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

        var payload = new ProductInputPayload { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, false, false), Times.Once());
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToUpdateAProduct()
    {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product);
        var service = BuildSubject(repository: mockRepository);

        var payload = new ProductInputPayload { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(m => m.UpdateAsync(product), Times.Once());
    }

    [Fact]
    public async Task Run_ReturnsAnObjectRepresentingTheNewlyUpdatedProduct()
    {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product);
        var service = BuildSubject(repository: mockRepository);

        var payload = new ProductInputPayload { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        var result = await service.Run(payload);

        // Assert
        Assert.Equal("test_name", result.Name);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Run_ThrowsAnEntityNotFoundException_WhenTheProductCannotBeFound()
    {
        // Arrange
        var service = BuildSubject();

        var payload = new ProductInputPayload { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Run(payload));
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        await Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedImageFile(
            (subject, file) => subject.MainImageFile = file
        );
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedThumbnailImageFile()
    {
        await Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedImageFile(
            (subject, file) => subject.ThumbnailImageFile = file
        );
    }

    private async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedImageFile(
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner
    ) {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        mockFormFileAssigner(payload, mockFormFile.Object);

        // Act
        await service.Run(payload);

        // Assert
        mockFileService.Verify(
            m => m.SaveFile(mockFormFile.Object, "mock_product_images_path"),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_SetsTheMainImageOnTheProductToUpdate_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        await Run_SetsTheImageOnTheProductToUpdate_WhenThePayloadIncludesAnUploadedImageFile(
            (payload, file) => payload.MainImageFile = file,
            c => c.MainImage != null
        );
    }

    [Fact]
    public async Task Run_SetsTheThumbnailImageOnTheProductToUpdate_WhenThePayloadIncludesAnUploadedThumbnailImageFile()
    {
        await Run_SetsTheImageOnTheProductToUpdate_WhenThePayloadIncludesAnUploadedImageFile(
            (payload, file) => payload.ThumbnailImageFile = file,
            c => c.ThumbnailImage != null
        );
    }

    private async Task Run_SetsTheImageOnTheProductToUpdate_WhenThePayloadIncludesAnUploadedImageFile(
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner,
        Expression<Func<Product, bool>> updateAsyncParamChecker
    ) {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        mockFormFileAssigner(payload, mockFormFile.Object);

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(
            m => m.UpdateAsync(It.Is(updateAsyncParamChecker)),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToDeleteAFile_WhenThePayloadIncludesAnUploadedMainImageFile_AndTheProductToUpdateAlreadyHasAMainImage()
    {
        await Run_CallsOnTheFileServiceToDeleteAFile_WhenThePayloadIncludesAnUploadedImageFile_AndTheProductToUpdateAlreadyHasAnImage(
            (product, image) => product.MainImage = image,
            (payload, file) => payload.MainImageFile = file
        );
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToDeleteAFile_WhenThePayloadIncludesAnUploadedThumbnailImageFile_AndTheProductToUpdateAlreadyHasAThumbnailImage()
    {
        await Run_CallsOnTheFileServiceToDeleteAFile_WhenThePayloadIncludesAnUploadedImageFile_AndTheProductToUpdateAlreadyHasAnImage(
            (product, image) => product.ThumbnailImage = image,
            (payload, file) => payload.ThumbnailImageFile = file
        );
    }

    private async Task Run_CallsOnTheFileServiceToDeleteAFile_WhenThePayloadIncludesAnUploadedImageFile_AndTheProductToUpdateAlreadyHasAnImage(
        Action<Product, ProductImage> existingImageAssigner,
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner
    ) {
        // Arrange
        var product = new Product()
        {
            Id = 10,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        existingImageAssigner(product, new() { FileName = "test_image_file_name" });

        var mockRepository = BuildMockRepository(product);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        mockFormFileAssigner(payload, mockFormFile.Object);

        // Act
        await service.Run(payload);

        // Assert
        mockFileService.Verify(
            m => m.DeleteFile("mock_product_images_path/test_image_file_name"),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToDeleteTheProductMainImageRecord_WhenThePayloadIncludesAnUploadedMainImageFile_AndTheProductToUpdateAlreadyHasAMainImage()
    {
        await Run_CallsOnTheRepositoryToDeleteTheProductImageRecord_WhenThePayloadIncludesAnUploadedImageFile_AndTheProductToUpdateAlreadyHasAnImage(
            (product, image) => product.MainImage = image,
            (payload, file) => payload.MainImageFile = file,
            (mockRepository, product) => mockRepository.Verify(m => m.DeleteMainImage(product), Times.Once())
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToDeleteTheProductThumbnailImageRecord_WhenThePayloadIncludesAnUploadedThumbnailImageFile_AndTheProductToUpdateAlreadyHasAThumbnailImage()
    {
        await Run_CallsOnTheRepositoryToDeleteTheProductImageRecord_WhenThePayloadIncludesAnUploadedImageFile_AndTheProductToUpdateAlreadyHasAnImage(
            (product, image) => product.ThumbnailImage = image,
            (payload, file) => payload.ThumbnailImageFile = file,
            (mockRepository, product) => mockRepository.Verify(m => m.DeleteThumbnailImage(product), Times.Once())
        );
    }

    private async Task Run_CallsOnTheRepositoryToDeleteTheProductImageRecord_WhenThePayloadIncludesAnUploadedImageFile_AndTheProductToUpdateAlreadyHasAnImage(
        Action<Product, ProductImage> existingImageAssigner,
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner,
        Action<Mock<IProductRepository>, Product> assert
    ) {
        // Arrange
        var product = new Product()
        {
            Id = 10,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        existingImageAssigner(product, new() { FileName = "test_image_file_name" });

        var mockRepository = BuildMockRepository(product);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        mockFormFileAssigner(payload, mockFormFile.Object);

        // Act
        await service.Run(payload);

        // Assert
        assert(mockRepository, product);
    }

    // This test case validates the scenario when the record being updated is
    // deleted from the database after it's been retrieved but before being
    // updated. Normally it's deleted by another process or thread.
    [Fact]
    public async Task Run_ThrowsAnEntityNotFoundException_WhenTheCallToTheRepositoryToUpdateThrowsAnException_AndTheProductBeingUpdatedDoesNotExistAnymore()
    {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product, exists: false);

        mockRepository
            .Setup(m => m.UpdateAsync(It.IsAny<Product>()))
            .Throws<DbUpdateConcurrencyException>();

        var service = BuildSubject(repository: mockRepository);

        var payload = new ProductInputPayload { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Run(payload));
    }

    [Fact]
    public async Task Run_ThrowsAnException_WhenTheCallToTheRepositoryToUpdateThrowsAnException_AndTheProductBeingUpdatedExists()
    {
        // Arrange
        var product = new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockRepository(product, exists: true);

        mockRepository
            .Setup(m => m.UpdateAsync(It.IsAny<Product>()))
            .Throws<DbUpdateConcurrencyException>();

        var service = BuildSubject(repository: mockRepository);

        var payload = new ProductInputPayload { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act && Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.Run(payload));
    }
}