using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Validation;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Validation;

public class UniqueProductUrlKeyAttributeTests : BaseUniqueFieldAttributeTests<Product, IProductRepository>
{
    protected override UniqueProductUrlKeyAttribute BuildTestSubject() => new();

    protected override Product BuildEntity() =>
        new() { Sku = "test_sku", Name = "test_name", BasePrice = 0, UrlKey = "test_url_key" };

    protected override Mock<IProductRepository> MockEntityRepository(object? returnValue)
    {
        var mockProductRepository = new Mock<IProductRepository>();

        mockProductRepository
            .Setup(m => m.FindByUrlKeyAsync("test_url_key", false))
            .ReturnsAsync((Product?)returnValue);

        return mockProductRepository;
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAnotherProductExistsWithTheGivenUrlKey()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var aProduct = BuildEntity();
        aProduct.Id = 10;

        var anotherProduct = BuildEntity();
        anotherProduct.Id = 20;

        var mockProductRepository = MockEntityRepository(aProduct);
        var mockServiceProvider = MockServiceProvider(mockProductRepository.Object);

        var context = new ValidationContext(anotherProduct, mockServiceProvider.Object, null);

        // Act
        var result = attribute.GetValidationResult("test_url_key", context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("The product URL Key 'test_url_key' is already in use.", result?.ErrorMessage);
    }
}
