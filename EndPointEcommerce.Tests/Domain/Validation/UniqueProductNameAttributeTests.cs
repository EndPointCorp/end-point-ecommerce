using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Validation;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Validation;

public class UniqueProductNameAttributeTests : BaseUniqueFieldAttributeTests<Product, IProductRepository>
{
    protected override UniqueProductNameAttribute BuildTestSubject() => new();

    protected override Product BuildEntity() =>
        new() { Sku = "test_sku", Name = "test_name", BasePrice = 0 };

    protected override Mock<IProductRepository> MockEntityRepository(object? returnValue)
    {
        var mockProductRepository = new Mock<IProductRepository>();

        mockProductRepository
            .Setup(m => m.FindByNameAsync("test_name"))
            .ReturnsAsync((Product?)returnValue);

        return mockProductRepository;
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAnotherProductExistsWithTheGivenName()
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
        var result = attribute.GetValidationResult("test_name", context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("The product name 'test_name' is already in use.", result?.ErrorMessage);
    }
}
