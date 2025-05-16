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

namespace EndPointCommerce.Tests.Domain.Services;

public class CategoryUpdaterTests
{
    private static Mock<ICategoryRepository> BuildMockRepository(Category? category = null, bool exists = true)
    {
        var mockRepository = new Mock<ICategoryRepository>();

        mockRepository
            .Setup(m => m.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(category);

        mockRepository
            .Setup(m => m.ExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(exists);

        mockRepository
            .Setup(m => m.UpdateAsync(It.IsAny<Category>()));

        mockRepository
            .Setup(m => m.DeleteMainImage(It.IsAny<Category>()));

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
            .Setup(m => m["CategoryImagesPath"])
            .Returns("mock_category_images_path");

        return mockConfiguration;
    }

    private static CategoryUpdater BuildSubject(
        Mock<ICategoryRepository>? repository = null,
        Mock<IFileService>? fileService = null,
        Mock<IConfiguration>? configuration = null
    ) {
        var mockRepository = repository ?? BuildMockRepository();
        var mockFileService = fileService ?? BuildMockFileService();
        var mockConfiguration = configuration ?? BuildMockConfiguration();

        return new CategoryUpdater(
            mockRepository.Object,
            mockFileService.Object,
            mockConfiguration.Object
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToFindACategory()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload { Id = 10, Name = "test_name" };

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, false), Times.Once());
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToUpdateACategory()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload { Id = 10, Name = "test_name" };

        // Act
        await service.Run(payload);

        // Assert
        mockRepository.Verify(m => m.UpdateAsync(category), Times.Once());
    }

    [Fact]
    public async Task Run_ReturnsAnObjectRepresentingTheNewlyUpdatedCategory()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload { Id = 10, Name = "test_name" };

        // Act
        var result = await service.Run(payload);

        // Assert
        Assert.Equal("test_name", result.Name);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Run_ThrowsAnEntityNotFoundException_WhenTheCategoryCannotBeFound()
    {
        // Arrange
        var service = BuildSubject();

        var payload = new CategoryInputPayload { Id = 10, Name = "test_name" };

        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Run(payload));
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToSaveAFile_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

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
    public async Task Run_SetsTheMainImageOnTheCategoryToUpdate_WhenThePayloadIncludesAnUploadedMainImageFile()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

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
            m => m.UpdateAsync(It.Is(includingTheMainImage)),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToDeleteAFile_WhenThePayloadIncludesAnUploadedMainImageFile_AndTheCategoryToUpdateAlreadyHasAMainImage()
    {
        // Arrange
        var category = new Category()
        {
            Id = 10,
            Name = "test_name",
            MainImage = new() { FileName = "test_main_image_file_name" }
        };

        var mockRepository = BuildMockRepository(category);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

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
            m => m.DeleteFile("mock_category_images_path/test_main_image_file_name"),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToDeleteTheCategoryMainImageRecord_WhenThePayloadIncludesAnUploadedMainImageFile_AndTheCategoryToUpdateAlreadyHasAMainImage()
    {
        // Arrange
        var category = new Category()
        {
            Id = 10,
            Name = "test_name",
            MainImage = new() { FileName = "test_main_image_file_name" }
        };

        var mockRepository = BuildMockRepository(category);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

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
        mockRepository.Verify(m => m.DeleteMainImage(category), Times.Once());
    }

    // This test case validates the scenario when the record being updated is
    // deleted from the database after it's been retrieved but before being
    // updated. Normally it's deleted by another process or thread.
    [Fact]
    public async Task Run_ThrowsAnEntityNotFoundException_WhenTheCallToTheRepositoryToUpdateThrowsAnException_AndTheCategoryBeingUpdatedDoesNotExistAnymore()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category, exists: false);

        mockRepository
            .Setup(m => m.UpdateAsync(It.IsAny<Category>()))
            .Throws<DbUpdateConcurrencyException>();

        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload { Id = 10, Name = "test_name" };

        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Run(payload));
    }

    [Fact]
    public async Task Run_ThrowsAnException_WhenTheCallToTheRepositoryToUpdateThrowsAnException_AndTheCategoryBeingUpdatedExists()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category, exists: true);

        mockRepository
            .Setup(m => m.UpdateAsync(It.IsAny<Category>()))
            .Throws<DbUpdateConcurrencyException>();

        var service = BuildSubject(repository: mockRepository);

        var payload = new CategoryInputPayload { Id = 10, Name = "test_name" };

        // Act && Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.Run(payload));
    }
}