// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Services;

public class CategoryMainImageDeleterTests
{
    private static Mock<ICategoryRepository> BuildMockRepository(Category? category = null)
    {
        var mockRepository = new Mock<ICategoryRepository>();

        mockRepository
            .Setup(m => m.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(category);

        mockRepository
            .Setup(m => m.DeleteMainImage(It.IsAny<Category>()));

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
            .Setup(m => m["CategoryImagesPath"])
            .Returns("mock_category_images_path");

        return mockConfiguration;
    }

    private static CategoryMainImageDeleter BuildSubject(
        Mock<ICategoryRepository>? repository = null,
        Mock<IFileService>? fileService = null,
        Mock<IConfiguration>? configuration = null
    ) {
        var mockRepository = repository ?? BuildMockRepository();
        var mockFileService = fileService ?? BuildMockFileService();
        var mockConfiguration = configuration ?? BuildMockConfiguration();

        return new CategoryMainImageDeleter(
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

        // Act
        await service.Run(10);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, false), Times.Once());
    }

    [Fact]
    public async Task Run_ReturnsAnObjectRepresentingTheNewlyUpdatedCategory()
    {
        // Arrange
        var category = new Category() { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var service = BuildSubject(repository: mockRepository);

        // Act
        var result = await service.Run(10);

        // Assert
        Assert.Equal("test_name", result.Name);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Run_ThrowsAnEntityNotFoundException_WhenTheCategoryCannotBeFound()
    {
        // Arrange
        var service = BuildSubject();

        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Run(10));
    }

    [Fact]
    public async Task Run_CallsOnTheFileServiceToDeleteAFile_WhenTheCategoryToUpdateHasAMainImage()
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

        // Act
        await service.Run(10);

        // Assert
        mockFileService.Verify(
            m => m.DeleteFile("mock_category_images_path/test_main_image_file_name"),
            Times.Once()
        );
    }

    [Fact]
    public async Task Run_CallsOnTheRepositoryToDeleteTheCategoryMainImageRecord_WhenTheCategoryToUpdateHasAMainImage()
    {
        // Arrange
        var category = new Category()
        {
            Id = 10,
            Name = "test_name",
            MainImage = new() { FileName = "test_main_image_file_name" }
        };

        var mockRepository = BuildMockRepository(category);
        var service = BuildSubject(repository: mockRepository);

        // Act
        await service.Run(10);

        // Assert
        mockRepository.Verify(m => m.DeleteMainImage(category), Times.Once());
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheFileServiceToDeleteAFile_WhenTheCategoryToUpdateDoesNotHaveAMainImage()
    {
        // Arrange
        var category = new Category()
        {
            Id = 10,
            Name = "test_name",
            MainImage = null
        };

        var mockRepository = BuildMockRepository(category);
        var mockFileService = BuildMockFileService();
        var service = BuildSubject(repository: mockRepository, fileService: mockFileService);

        // Act
        await service.Run(10);

        // Assert
        mockFileService.Verify(m => m.DeleteFile(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task Run_DoesNotCallOnTheRepositoryToDeleteTheCategoryMainImageRecord_WhenTheCategoryToUpdateDoesNotHaveAMainImage()
    {
        // Arrange
        var category = new Category()
        {
            Id = 10,
            Name = "test_name",
            MainImage = null
        };

        var mockRepository = BuildMockRepository(category);
        var service = BuildSubject(repository: mockRepository);

        // Act
        await service.Run(10);

        // Assert
        mockRepository.Verify(m => m.DeleteMainImage(It.IsAny<Category>()), Times.Never());
    }
}