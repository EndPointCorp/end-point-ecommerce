using System.Linq.Expressions;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Services;

public class CategoryCreatorTests
{
    private static Mock<ICategoryRepository> BuildMockRepository()
    {
        var mockRepository = new Mock<ICategoryRepository>();
        mockRepository
            .Setup(m => m.AddAsync(It.IsAny<Category>()))
            // The repo is expected to assign the db-generated id to the newly created record
            .Callback<Category>(c => c.Id = 10);

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
            .Setup(m => m["CategoryImagesPath"])
            .Returns("mock_category_images_path");

        return mockConfiguration;
    }

    private static CategoryCreator BuildSubject(
        Mock<ICategoryRepository>? repository = null,
        Mock<IFileService>? fileService = null,
        Mock<IConfiguration>? configuration = null
    ) {
        var mockRepository = repository ?? BuildMockRepository();
        var mockFileService = fileService ?? BuildMockFileService();
        var mockConfiguration = configuration ?? BuildMockConfiguration();

        return new CategoryCreator(
            mockRepository.Object,
            mockFileService.Object,
            mockConfiguration.Object
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToCreateACategory()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload { Name = "test_name" };

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(m => m.AddAsync(It.IsAny<Category>()), Times.Once());
    }

    [Fact]
    public async Task Run_ReturnsAnObjectRepresentingTheNewlyCreatedCategory()
    {
        // Arrange
        var service = BuildSubject();

        var payload = new CategoryInputPayload { Name = "test_name" };

        // Act
        var result = await service.Run(payload);

        // Assert
        Assert.Equal("test_name", result.Name);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        // Arrange
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(fileService: mockFileService);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new CategoryInputPayload
        {
            Name = "test_name",
            MainImageFile = mockFormFile.Object
        };

        // Act
        await service.Run(payload);

        // Assert
        mockFileService.Verify(
            m => m.SaveFile(mockFormFile.Object, "mock_category_images_path"),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_SetsTheMainImageOnTheCategoryToCreate_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new CategoryInputPayload
        {
            Name = "test_name",
            MainImageFile = mockFormFile.Object
        };

        // Act
        await service.Run(payload);

        // Assert
        Expression<Func<Category, bool>> includingTheMainImage = c => c.MainImage != null;

        mockRepository.Verify(
            m => m.AddAsync(It.Is(includingTheMainImage)),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_SetsTheTheFileNameOnTheMainImageOnTheCategoryToCreateToTheFileNameGivenByTheFileService()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile
            .Setup(m => m.Length)
            .Returns(123);

        var payload = new CategoryInputPayload
        {
            Name = "test_name",
            MainImageFile = mockFormFile.Object
        };

        // Act
        await service.Run(payload);

        // Assert
        Expression<Func<Category, bool>> includingTheMainImageFileName = c => c.MainImage!.FileName == "test_file_name";

        mockRepository.Verify(
            m => m.AddAsync(It.Is(includingTheMainImageFileName)),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheFileServiceToSaveAFile_WhenThePayloadDoesNotIncludeAnUploadedMainImageFile()
    {
        // Arrange
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(fileService: mockFileService);

        var payload = new CategoryInputPayload
        {
            Name = "test_name",
            MainImageFile = null
        };

        // Act
        await service.Run(payload);

        // Assert
        mockFileService.Verify(
            m => m.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()),
            Times.Never()
        );
    }

    [Fact]
    public async Task Run_DoesNotSetTheMainImageOnTheCategoryToCreate_WhenThePayloadDoesNotIncludeAnUploadedMainImageFile()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload
        {
            Name = "test_name",
            MainImageFile = null
        };

        // Act
        await service.Run(payload);

        // Assert
        Expression<Func<Category, bool>> notIncludingTheMainImage = c => c.MainImage == null;

        mockRepository.Verify(
            m => m.AddAsync(It.Is(notIncludingTheMainImage)),
            Times.Once()
        );
    }
}