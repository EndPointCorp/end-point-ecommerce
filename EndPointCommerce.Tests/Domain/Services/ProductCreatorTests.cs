using System.Linq.Expressions;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EndPointCommerce.Tests.Domain.Services;

public class ProductCreatorTests
{
    private static Mock<IProductRepository> BuildMockRepository()
    {
        var mockRepository = new Mock<IProductRepository>();
        mockRepository
            .Setup(m => m.AddAsync(It.IsAny<Product>()))
            // The repo is expected to assign the db-generated id to the newly created record
            .Callback<Product>(c => c.Id = 10);

        return mockRepository;
    }

    private static Mock<IFileService> BuildMockFileService()
    {
        var mockFileService = new Mock<IFileService>();
        mockFileService
            .Setup(m => m.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync("test_file_name");

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

    private static ProductCreator BuildSubject(
        Mock<IProductRepository>? repository = null,
        Mock<IFileService>? fileService = null,
        Mock<IConfiguration>? configuration = null
    ) {
        var mockRepository = repository ?? BuildMockRepository();
        var mockFileService = fileService ?? BuildMockFileService();
        var mockConfiguration = configuration ?? BuildMockConfiguration();

        return new ProductCreator(
            mockRepository.Object,
            mockFileService.Object,
            mockConfiguration.Object
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToCreateAProduct()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var payload = new ProductInputPayload { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(m => m.AddAsync(It.IsAny<Product>()), Times.Once());
    }

    [Fact]
    public async Task Run_ReturnsAnObjectRepresentingTheNewlyCreatedProduct()
    {
        // Arrange
        var service = BuildSubject();

        var payload = new ProductInputPayload { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        var result = await service.Run(payload);

        // Assert
        Assert.Equal("test_name", result.Name);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        await Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedImageFile(
            (payload, file) => payload.MainImageFile = file
        );
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedThumbnailImageFile()
    {
        await Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedImageFile(
            (payload, file) => payload.ThumbnailImageFile = file
        );
    }

    private async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedImageFile(
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner
    ) {
        // Arrange
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(fileService: mockFileService);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
            // ThumbnailImageFile = mockFormFile.Object
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
    public async Task Run_SetsTheMainImageOnTheProductToCreate_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        await Run_SetsTheImageOnTheProductToCreate_WhenThePayloadIncludesAnUploadedImageFile(
            (payload, file) => payload.MainImageFile = file,
            c => c.MainImage != null
        );
    }

    [Fact]
    public async Task Run_SetsTheThumbnailImageOnTheProductToCreate_WhenThePayloadIncludesAnUploadedThumbnailImageFile()
    {
        await Run_SetsTheImageOnTheProductToCreate_WhenThePayloadIncludesAnUploadedImageFile(
            (payload, file) => payload.ThumbnailImageFile = file,
            c => c.ThumbnailImage != null
        );
    }

    private async Task Run_SetsTheImageOnTheProductToCreate_WhenThePayloadIncludesAnUploadedImageFile(
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner,
        Expression<Func<Product, bool>> addAsyncParamChecker
    ) {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M,
        };

        mockFormFileAssigner(payload, mockFormFile.Object);

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(
            m => m.AddAsync(It.Is(addAsyncParamChecker)),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_SetsTheTheFileNameOnTheMainImageOnTheProductToCreateToTheFileNameGivenByTheFileService()
    {
        await Run_SetsTheTheFileNameOnTheImageOnTheProductToCreateToTheFileNameGivenByTheFileService(
            (payload, file) => payload.MainImageFile = file,
            c => c.MainImage!.FileName == "test_file_name"
        );
    }

    [Fact]
    public async Task Run_SetsTheTheFileNameOnTheThumbnailImageOnTheProductToCreateToTheFileNameGivenByTheFileService()
    {
        await Run_SetsTheTheFileNameOnTheImageOnTheProductToCreateToTheFileNameGivenByTheFileService(
            (payload, file) => payload.ThumbnailImageFile = file,
            c => c.ThumbnailImage!.FileName == "test_file_name"
        );
    }

    private async Task Run_SetsTheTheFileNameOnTheImageOnTheProductToCreateToTheFileNameGivenByTheFileService(
        Action<ProductInputPayload, IFormFile> mockFormFileAssigner,
        Expression<Func<Product, bool>> addAsyncParamChecker
    ) {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

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
            m => m.AddAsync(It.Is(addAsyncParamChecker)),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheFileServiceToSaveAFile_WhenThePayloadDoesNotIncludeAnUploadedMainImageFile()
    {
        await Run_DoesNotCallOnTheFileServiceToSaveAFile_WhenThePayloadDoesNotIncludeAnUploadedImageFile(
            (payload) => payload.MainImageFile = null
        );
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheFileServiceToSaveAFile_WhenThePayloadDoesNotIncludeAnUploadedThumbnailImageFile()
    {
        await Run_DoesNotCallOnTheFileServiceToSaveAFile_WhenThePayloadDoesNotIncludeAnUploadedImageFile(
            (payload) => payload.ThumbnailImageFile = null
        );
    }

    private async Task Run_DoesNotCallOnTheFileServiceToSaveAFile_WhenThePayloadDoesNotIncludeAnUploadedImageFile(
        Action<ProductInputPayload> nullifier
    ) {
        // Arrange
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(fileService: mockFileService);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        nullifier(payload);

        // Act
        await service.Run(payload);

        // Assert
        mockFileService.Verify(
            m => m.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()),
            Times.Never()
        );
    }

    [Fact]
    public async Task Run_DoesNotSetTheMainImageOnTheProductToCreate_WhenThePayloadDoesNotIncludeAnUploadedMainImageFile()
    {
        await Run_DoesNotSetTheImageOnTheProductToCreate_WhenThePayloadDoesNotIncludeAnUploadedImageFile(
            (payload) => payload.MainImageFile = null,
            c => c.MainImage == null
        );
    }

    [Fact]
    public async Task Run_DoesNotSetTheThumbnailImageOnTheProductToCreate_WhenThePayloadDoesNotIncludeAnUploadedThumbnailImageFile()
    {
        await Run_DoesNotSetTheImageOnTheProductToCreate_WhenThePayloadDoesNotIncludeAnUploadedImageFile(
            (payload) => payload.ThumbnailImageFile = null,
            c => c.ThumbnailImage == null
        );
    }

    private async Task Run_DoesNotSetTheImageOnTheProductToCreate_WhenThePayloadDoesNotIncludeAnUploadedImageFile(
        Action<ProductInputPayload> nullifier,
        Expression<Func<Product, bool>> addAsyncParamChecker
    ) {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var payload = new ProductInputPayload
        {
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        nullifier(payload);

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(
            m => m.AddAsync(It.Is(addAsyncParamChecker)),
            Times.Once()
        );
    }
}