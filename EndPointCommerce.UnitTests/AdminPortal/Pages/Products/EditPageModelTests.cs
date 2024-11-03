using EndPointCommerce.AdminPortal.Pages.Products;
using EndPointCommerce.AdminPortal.ViewModels;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace EndPointCommerce.UnitTests.AdminPortal.Pages.Products;

public class EditPageModelTests
{
    private static Mock<IProductUpdater> BuildMockProductUpdater()
    {
        var mockProductUpdater = new Mock<IProductUpdater>();
        mockProductUpdater
            .Setup(m => m.Run(It.IsAny<ProductInputPayload>()))
            // The updater is expected to return an object with the record's new state.
            .ReturnsAsync(new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M });

        return mockProductUpdater;
    }

    private static Mock<IProductMainImageDeleter> BuildMockProductMainImageDeleter()
    {
        var mockProductMainImageDeleter = new Mock<IProductMainImageDeleter>();
        mockProductMainImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .ReturnsAsync(new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M });

        return mockProductMainImageDeleter;
    }

    private static Mock<IProductThumbnailImageDeleter> BuildMockProductThumbnailImageDeleter()
    {
        var mockProductThumbnailImageDeleter = new Mock<IProductThumbnailImageDeleter>();
        mockProductThumbnailImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .ReturnsAsync(new Product() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M });

        return mockProductThumbnailImageDeleter;
    }

    private static Mock<IProductAdditionalImageDeleter> BuildMockProductAdditionalImageDeleter()
    {
        var mockProductAdditionalImageDeleter = new Mock<IProductAdditionalImageDeleter>();
        return mockProductAdditionalImageDeleter;
    }

    private static Mock<IProductRepository> BuildMockProductRepository(Product? product = null)
    {
        var mockRepository = new Mock<IProductRepository>();

        mockRepository
            .Setup(m => m.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(product);

        return mockRepository;
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

    private static EditModel BuildSubject(
        Mock<IProductUpdater>? productUpdater = null,
        Mock<IProductMainImageDeleter>? productMainImageDeleter = null,
        Mock<IProductThumbnailImageDeleter>? productThumbnailImageDeleter = null,
        Mock<IProductAdditionalImageDeleter>? productAdditionalImageDeleter = null,
        Mock<IProductRepository>? productRepository = null,
        Mock<ICategoryRepository>? categoryRepository = null
    ) {
        var mockProductUpdater = productUpdater ?? BuildMockProductUpdater();
        var mockProductMainImageDeleter = productMainImageDeleter ?? BuildMockProductMainImageDeleter();
        var mockProductThumbnailImageDeleter = productThumbnailImageDeleter ?? BuildMockProductThumbnailImageDeleter();
        var mockProductAdditionalImageDeleter = productAdditionalImageDeleter ?? BuildMockProductAdditionalImageDeleter();
        var mockProductRepository = productRepository ?? BuildMockProductRepository();
        var mockCategoryRepository = categoryRepository ?? BuildMockCategoryRepository();

        var subject = new EditModel(
            mockProductUpdater.Object,
            mockProductMainImageDeleter.Object,
            mockProductThumbnailImageDeleter.Object,
            mockProductAdditionalImageDeleter.Object,
            mockProductRepository.Object,
            mockCategoryRepository.Object
        ) {
            Product = new() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M }
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
    public async Task OnGetAsync_CallsOnTheRepositoryToFindTheProductById_WhenGivenAnId()
    {
        // Arrange
        var mockRepository = BuildMockProductRepository();
        var pageModel = BuildSubject(productRepository: mockRepository);

        // Act
        var result = await pageModel.OnGetAsync(10);

        // Assert
        mockRepository.Verify(m => m.FindByIdAsync(10, false, false), Times.Once());
    }

    [Fact]
    public async Task OnGetAsync_ReturnsNotFound_WhenTheProductIsNotFound()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnGetAsync(10);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_PopulatesTheFoundProduct_WhenTheProductIsFound()
    {
        // Arrange
        var product = new Product { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockProductRepository(product);
        var pageModel = BuildSubject(productRepository: mockRepository);

        // Act
        await pageModel.OnGetAsync(10);

        // Assert
        Assert.Equal(product.Id, pageModel.Product.Id);
        Assert.Equal(product.Name, pageModel.Product.Name);
    }

    [Fact]
    public async Task OnGetAsync_PopulatesTheCategoryOptions_WhenTheProductIsFound()
    {
        // Arrange
        var product = new Product { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockProductRepository(product);
        var pageModel = BuildSubject(productRepository: mockRepository);

        // Act
        await pageModel.OnGetAsync(10);

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
    public async Task OnGetAsync_RendersThePage_WhenTheProductIsFound()
    {
        // Arrange
        var product = new Product { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };
        var mockRepository = BuildMockProductRepository(product);
        var pageModel = BuildSubject(productRepository: mockRepository);

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
        pageModel.Product = ProductViewModel.CreateDefault();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_ReturnsThePage_WhenThereIsAModelValidationError()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.Product = ProductViewModel.CreateDefault();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_PopulatesTheCategoryOptions_WhenThereIsAModelValidationError()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.Product = ProductViewModel.CreateDefault();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAsync();

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
    public async Task OnPostSaveAndContinueAsync_PopulatesTheCategoryOptions_WhenThereIsAModelValidationError()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.Product = ProductViewModel.CreateDefault();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

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
    public async Task OnPostSaveAsync_DoesNotRunTheProductUpdater_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockRepository = BuildMockProductRepository();
        var pageModel = BuildSubject(productRepository: mockRepository);
        pageModel.Product = ProductViewModel.CreateDefault();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockRepository.Verify(m => m.UpdateAsync(It.IsAny<Product>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_DoesNotRunTheCategoryUpdater_WhenThereIsAModelValidationError()
    {
        // Arrange
        var mockRepository = BuildMockProductRepository();
        var pageModel = BuildSubject(productRepository: mockRepository);
        pageModel.Product = ProductViewModel.CreateDefault();
        pageModel.ModelState.AddModelError("test_error", "test_error");

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockRepository.Verify(m => m.UpdateAsync(It.IsAny<Product>()), Times.Never());
    }

    [Fact]
    public async Task OnPostSaveAsync_RunsTheProductUpdater()
    {
        // Arrange
        var mockProductUpdater = BuildMockProductUpdater();
        var pageModel = BuildSubject(productUpdater: mockProductUpdater);

        pageModel.Product = new() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostSaveAsync();

        // Assert
        mockProductUpdater.Verify(m => m.Run(It.IsAny<ProductInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RunsTheProductUpdater()
    {
        // Arrange
        var mockProductUpdater = BuildMockProductUpdater();
        var pageModel = BuildSubject(productUpdater: mockProductUpdater);

        pageModel.Product = new() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        mockProductUpdater.Verify(m => m.Run(It.IsAny<ProductInputPayload>()), Times.Once());
    }

    [Fact]
    public async Task OnPostSaveAsync_RedirectsToTheIndexPage()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.Product = ProductViewModel.CreateDefault();

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_RendersThePage()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.Product = ProductViewModel.CreateDefault();

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_PopulatesTheCategoryOptions()
    {
        // Arrange
        var pageModel = BuildSubject();
        pageModel.Product = new() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostSaveAndContinueAsync();

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
    public async Task OnPostSaveAsync_ReturnsNotFound_WhenTheProductUpdaterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockProductUpdater = BuildMockProductUpdater();
        mockProductUpdater
            .Setup(m => m.Run(It.IsAny<ProductInputPayload>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(productUpdater: mockProductUpdater);

        // Act
        var result = await pageModel.OnPostSaveAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_ReturnsNotFound_WhenTheProductUpdaterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockProductUpdater = BuildMockProductUpdater();
        mockProductUpdater
            .Setup(m => m.Run(It.IsAny<ProductInputPayload>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(productUpdater: mockProductUpdater);

        // Act
        var result = await pageModel.OnPostSaveAndContinueAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostSaveAsync_ThrowsAnException_WhenTheProductUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockProductUpdater = BuildMockProductUpdater();
        mockProductUpdater
            .Setup(m => m.Run(It.IsAny<ProductInputPayload>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(productUpdater: mockProductUpdater);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostSaveAsync);
    }

    [Fact]
    public async Task OnPostSaveAndContinueAsync_ThrowsAnException_WhenTheProductUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockProductUpdater = BuildMockProductUpdater();
        mockProductUpdater
            .Setup(m => m.Run(It.IsAny<ProductInputPayload>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(productUpdater: mockProductUpdater);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostSaveAndContinueAsync);
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_RunsTheProductMainImageDeleter()
    {
        // Arrange
        var mockProductMainImageDeleter = BuildMockProductMainImageDeleter();
        var pageModel = BuildSubject(productMainImageDeleter: mockProductMainImageDeleter);

        pageModel.Product = new() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostDeleteMainImageAsync();

        // Assert
        mockProductMainImageDeleter.Verify(m => m.Run(10), Times.Once());
    }

    [Fact]
    public async Task OnPostDeleteThumbnailImageAsync_RunsTheProductThumbnailImageDeleter()
    {
        // Arrange
        var mockProductThumbnailImageDeleter = BuildMockProductThumbnailImageDeleter();
        var pageModel = BuildSubject(productThumbnailImageDeleter: mockProductThumbnailImageDeleter);

        pageModel.Product = new() { Id = 10, Name = "test_name", Sku = "test_sku", BasePrice = 10.00M };

        // Act
        await pageModel.OnPostDeleteThumbnailImageAsync();

        // Assert
        mockProductThumbnailImageDeleter.Verify(m => m.Run(10), Times.Once());
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
    public async Task OnPostDeleteThumbnailImageAsync_RendersThePage()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnPostDeleteThumbnailImageAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_PopulatesTheCategoryOptions()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnPostDeleteMainImageAsync();

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
    public async Task OnPostDeleteThumbnailImageAsync_PopulatesTheCategoryOptions()
    {
        // Arrange
        var pageModel = BuildSubject();

        // Act
        var result = await pageModel.OnPostDeleteThumbnailImageAsync();

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
    public async Task OnPostDeleteMainImageAsync_ReturnsNotFound_WhenTheProductMainImageDeleterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockProductMainImageDeleter = BuildMockProductMainImageDeleter();
        mockProductMainImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(productMainImageDeleter: mockProductMainImageDeleter);

        // Act
        var result = await pageModel.OnPostDeleteMainImageAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteThumbnailImageAsync_ReturnsNotFound_WhenTheProductThumbnailImageDeleterThrowsAnEntityNotFoundException()
    {
        // Arrange
        var mockProductThumbnailImageDeleter = BuildMockProductThumbnailImageDeleter();
        mockProductThumbnailImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .Throws<EntityNotFoundException>();

        var pageModel = BuildSubject(productThumbnailImageDeleter: mockProductThumbnailImageDeleter);

        // Act
        var result = await pageModel.OnPostDeleteThumbnailImageAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteMainImageAsync_ThrowsAnException_WhenTheProductUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockProductMainImageDeleter = BuildMockProductMainImageDeleter();
        mockProductMainImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(productMainImageDeleter: mockProductMainImageDeleter);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostDeleteMainImageAsync);
    }

    [Fact]
    public async Task OnPostDeleteThumbnailImageAsync_ThrowsAnException_WhenTheProductUpdaterThrowsAnUnexpectedException()
    {
        // Arrange
        var mockProductThumbnailImageDeleter = BuildMockProductThumbnailImageDeleter();
        mockProductThumbnailImageDeleter
            .Setup(m => m.Run(It.IsAny<int>()))
            .Throws<Exception>();

        var pageModel = BuildSubject(productThumbnailImageDeleter: mockProductThumbnailImageDeleter);

        // Act && Assert
        await Assert.ThrowsAsync<Exception>(pageModel.OnPostDeleteThumbnailImageAsync);
    }
}
