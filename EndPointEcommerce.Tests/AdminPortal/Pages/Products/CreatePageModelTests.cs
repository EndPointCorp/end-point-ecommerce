using EndPointEcommerce.AdminPortal.Pages.Products;
using EndPointEcommerce.AdminPortal.ViewModels;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace EndPointEcommerce.Tests.AdminPortal.Pages.Products;

public class CreatePageModelTests
{
    private static Mock<IProductCreator> BuildMockProductCreator()
    {
        var mockProductCreator = new Mock<IProductCreator>();
        mockProductCreator
            .Setup(m => m.Run(It.IsAny<ProductInputPayload>()))
            // The creator is expected to return an object with the db-generated id of the newly created record.
            .ReturnsAsync(new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M });

        return mockProductCreator;
    }

    private static Mock<ICategoryRepository> BuildMockCategoryRepository()
    {
        IList<Category> categories = [
            new() { Id = 10, Name = "test_name_1" },
            new() { Id = 20, Name = "test_name_2" }
        ];

        var mockRepository = new Mock<ICategoryRepository>();
        mockRepository
            .Setup(m => m.FetchAllAsync(false))
            .ReturnsAsync(categories);

        return mockRepository;
    }

    [Fact]
    public async Task OnGetAsync_RendersThePage()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object);

        // Act
        var result = await pageModel.OnGetAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_PopulatesTheCategoryOptions()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.NotNull(pageModel.Product.Categories);
        Assert.NotEmpty(pageModel.Product.Categories);
        Assert.Equal(2, pageModel.Product.Categories.Count());
        Assert.Equal("10", pageModel.Product.Categories.First().Value);
        Assert.Equal("test_name_1", pageModel.Product.Categories.First().Text);
        Assert.Equal("20", pageModel.Product.Categories.Last().Value);
        Assert.Equal("test_name_2", pageModel.Product.Categories.Last().Text);
    }

    [Fact]
    public async Task OnPostSaveAsync_ReturnsThePage_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object)
        {
            Product = ProductViewModel.CreateDefault()
        };

        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_DoesNotRunTheProductCreator_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object)
        {
            Product = ProductViewModel.CreateDefault()
        };

        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockProductCreator.Verify(m => m.Run(It.IsAny<ProductInputPayload>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_DoesNotRunTheProductCreator_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object)
        {
            Product = ProductViewModel.CreateDefault()
        };

        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockProductCreator.Verify(m => m.Run(It.IsAny<ProductInputPayload>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAsync_RunsTheProductCreator()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object);

        pageModel.Product = new() { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockProductCreator.Verify(m => m.Run(It.IsAny<ProductInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RunsTheProductCreator()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object);

        pageModel.Product = new() { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockProductCreator.Verify(m => m.Run(It.IsAny<ProductInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAsync_RedirectsToTheIndexPage()
    {
        // Arrange
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object)
        {
            Product = new() { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M }
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
        var mockProductCreator = BuildMockProductCreator();
        var mockCategoryRepository = BuildMockCategoryRepository();
        var pageModel = new CreateModel(mockProductCreator.Object, mockCategoryRepository.Object)
        {
            Product = new() { Name = "test_name", Sku = "test_sku", BasePrice = 10.00M }
        };

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Edit", ((RedirectToPageResult)result).PageName);
        Assert.Equal(10, ((RedirectToPageResult)result).RouteValues?["Id"]);
    }
}
