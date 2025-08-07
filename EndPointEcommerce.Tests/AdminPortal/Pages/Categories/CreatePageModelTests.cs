using EndPointEcommerce.AdminPortal.Pages.Categories;
using EndPointEcommerce.AdminPortal.ViewModels;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace EndPointEcommerce.Tests.AdminPortal.Pages.Categories;

public class CreatePageModelTests
{
    private static Mock<ICategoryCreator> BuildMockCategoryCreator()
    {
        var mockCategoryCreator = new Mock<ICategoryCreator>();
        mockCategoryCreator
            .Setup(m => m.Run(It.IsAny<CategoryInputPayload>()))
            // The creator is expected to return an object with the db-generated id of the newly created record.
            .ReturnsAsync(new Category() { Id = 10, Name = "test_name" });

        return mockCategoryCreator;
    }

    [Fact]
    public void OnGet_RendersThePage()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object);

        // Act
        var result = pageModel.OnGet();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_ReturnsThePage_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object);
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_DoesNotRunTheCategoryCreator_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object);
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockCategoryCreator.Verify(m => m.Run(It.IsAny<CategoryInputPayload>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_DoesNotRunTheCategoryCreator_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object);
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockCategoryCreator.Verify(m => m.Run(It.IsAny<CategoryInputPayload>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAsync_RunsTheCategoryCreator()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object)
        {
            Category = new CategoryViewModel { Name = "test_name" }
        };

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockCategoryCreator.Verify(m => m.Run(It.IsAny<CategoryInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RunsTheCategoryCreator()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object)
        {
            Category = new CategoryViewModel { Name = "test_name" }
        };

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockCategoryCreator.Verify(m => m.Run(It.IsAny<CategoryInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAsync_RedirectsToTheIndexPage()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object)
        {
            Category = new() { Name = "test_name" }
        };

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RedirectsToTheEditPage()
    {
        // Arrange
        var mockCategoryCreator = BuildMockCategoryCreator();
        var pageModel = new CreateModel(mockCategoryCreator.Object)
        {
            Category = new() { Name = "test_name" }
        };

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Edit", ((RedirectToPageResult)result).PageName);
        Assert.Equal(10, ((RedirectToPageResult)result).RouteValues?["Id"]);
    }
}
