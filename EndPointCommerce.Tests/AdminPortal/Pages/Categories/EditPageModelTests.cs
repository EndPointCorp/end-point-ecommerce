using EndPointCommerce.AdminPortal.Pages.Categories;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace EndPointCommerce.Tests.AdminPortal.Pages.Categories;

public class EditPageModelTests
{
    private static Mock<ICategoryUpdater> BuildMockCategoryUpdater()
    {
        var mockCategoryUpdater = new Mock<ICategoryUpdater>();
        mockCategoryUpdater
            .Setup(m => m.Run(It.IsAny<CategoryInputPayload>()))
            // The updater is expected to return an object with the record's new state.
            .ReturnsAsync(new Category() { Id = 10, Name = "test_name" });

        return mockCategoryUpdater;
    }

    private static Mock<ICategoryMainImageDeleter> BuildMockCategoryMainImageDeleter()
    {
        var mockCategoryMainImageDeleter = new Mock<ICategoryMainImageDeleter>();
        mockCategoryMainImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .ReturnsAsync(new Category() { Id = 10, Name = "test_name" });

        return mockCategoryMainImageDeleter;
    }

    private static Mock<ICategoryRepository> BuildMockRepository(Category? category = null, bool exists = true)
    {
        var mockRepository = new Mock<ICategoryRepository>();

        mockRepository
            .Setup(m => m.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(category);

        return mockRepository;
    }

    private static EditModel BuildSubject(
        Mock<ICategoryUpdater>? categoryUpdater = null,
        Mock<ICategoryMainImageDeleter>? categoryMainImageDeleter = null,
        Mock<ICategoryRepository>? repository = null
    ) {
        var mockCategoryUpdater = categoryUpdater ?? BuildMockCategoryUpdater();
        var mockCategoryMainImageDeleter = categoryMainImageDeleter ?? BuildMockCategoryMainImageDeleter();
        var mockRepository = repository ?? BuildMockRepository();

        var subject = new EditModel(
            mockCategoryUpdater.Object,
            mockCategoryMainImageDeleter.Object,
            mockRepository.Object
        ) {
            Category = new() { Id = 10, Name = "test_name" }
        };

        return subject;
    }

    [Fact]
    public async Task OnGetAsync_ReturnsNotFound_WhenGivenANullId()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnGetAsync(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_CallsOnTheRepositoryToFindTheCategoryById_WhenGivenAnId()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var pageModel = BuildSubject(repository: mockRepository);

        // Act
        var result = await pageModel.OnGetAsync(10);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, false), Times.Once());
    }

    [Fact]
    public async Task OnGetAsync_ReturnsNotFound_WhenTheCategoryIsNotFound()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnGetAsync(10);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_PopulatesTheFoundCategory_WhenTheCategoryIsFound()
    {
        // Arrange
        var category = new Category { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var pageModel = BuildSubject(repository: mockRepository);

        // Act
        await pageModel.OnGetAsync(10);

        // Assert
        Assert.Equal(category.Id, pageModel.Category.Id);
        Assert.Equal(category.Name, pageModel.Category.Name);
    }

    [Fact]
    public async Task OnGetAsync_RendersThePage_WhenTheCategoryIsFound()
    {
        // Arrange
        var category = new Category { Id = 10, Name = "test_name" };
        var mockRepository = BuildMockRepository(category);
        var pageModel = BuildSubject(repository: mockRepository);

        // Act
        var result = await pageModel.OnGetAsync(10);

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_ReturnsThePage_WhenThereIsAModelValidationError()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_DoesNotRunTheCategoryUpdater_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var pageModel = BuildSubject();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockRepository.Verify(m => m.UpdateAsync(It.IsAny<Category>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_DoesNotRunTheCategoryUpdater_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockRepository = BuildMockRepository();
        var pageModel = BuildSubject();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockRepository.Verify(m => m.UpdateAsync(It.IsAny<Category>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAsync_RunsTheCategoryUpdater()
    {
        // Arrange
        var mockCategoryUpdater = BuildMockCategoryUpdater();
        var pageModel = BuildSubject(categoryUpdater: mockCategoryUpdater);

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockCategoryUpdater.Verify(m => m.Run(It.IsAny<CategoryInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RunsTheCategoryUpdater()
    {
        // Arrange
        var mockCategoryUpdater = BuildMockCategoryUpdater();
        var pageModel = BuildSubject(categoryUpdater: mockCategoryUpdater);

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockCategoryUpdater.Verify(m => m.Run(It.IsAny<CategoryInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAsync_RedirectsToTheIndexPage()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RedirectsToThePage()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Edit", ((RedirectToPageResult)result).PageName);
        Assert.Equal(10, ((RedirectToPageResult)result).RouteValues!["Id"]);
    }

    [Fact]
    public async Task OnPostSaveAsync_ReturnsNotFound_WhenTheCategoryUpdaterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockCategoryUpdater = BuildMockCategoryUpdater();
        mockCategoryUpdater
            .Setup(m => m.Run(It.IsAny<CategoryInputPayload>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(categoryUpdater: mockCategoryUpdater);

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_ReturnsNotFound_WhenTheCategoryUpdaterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockCategoryUpdater = BuildMockCategoryUpdater();
        mockCategoryUpdater
            .Setup(m => m.Run(It.IsAny<CategoryInputPayload>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(categoryUpdater: mockCategoryUpdater);

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_ThrowsAnException_WhenTheCategoryUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockCategoryUpdater = BuildMockCategoryUpdater();
        mockCategoryUpdater
            .Setup(m => m.Run(It.IsAny<CategoryInputPayload>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(categoryUpdater: mockCategoryUpdater);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostSaveAsync);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_ThrowsAnException_WhenTheCategoryUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockCategoryUpdater = BuildMockCategoryUpdater();
        mockCategoryUpdater
            .Setup(m => m.Run(It.IsAny<CategoryInputPayload>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(categoryUpdater: mockCategoryUpdater);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostSaveAndContinueAsync);
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_RunsTheCategoryMainImageDeleter()
    {
        // Arrange
        var mockCategoryMainImageDeleter = BuildMockCategoryMainImageDeleter();
        var pageModel = BuildSubject(categoryMainImageDeleter: mockCategoryMainImageDeleter);

        // Act
        await pageModel.OnPostDeleteMainImageAsync();

        // Assert
        mockCategoryMainImageDeleter.Verify(m => m.Run(10), Times.Once());
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_RendersThePage()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnPostDeleteMainImageAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_ReturnsNotFound_WhenTheCategoryMainImageDeleterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockCategoryMainImageDeleter = BuildMockCategoryMainImageDeleter();
        mockCategoryMainImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(categoryMainImageDeleter: mockCategoryMainImageDeleter);

        // Act
        var result = await pageModel.OnPostDeleteMainImageAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_ThrowsAnException_WhenTheCategoryUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockCategoryMainImageDeleter = BuildMockCategoryMainImageDeleter();
        mockCategoryMainImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(categoryMainImageDeleter: mockCategoryMainImageDeleter);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostDeleteMainImageAsync);
    }
}
