using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Validation;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Validation;

public class UniqueCategoryNameAttributeTests : BaseUniqueFieldAttributeTests<Category, ICategoryRepository>
{
    protected override UniqueCategoryNameAttribute BuildTestSubject() => new();

    protected override Category BuildEntity() =>
        new() { Name = "test_name" };

    protected override Mock<ICategoryRepository> MockEntityRepository(object? returnValue)
    {
        var mockCategoryRepository = new Mock<ICategoryRepository>();

        mockCategoryRepository
            .Setup(m => m.FindByNameAsync("test_name"))
            .ReturnsAsync((Category?)returnValue);

        return mockCategoryRepository;
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAnotherCategoryExistsWithTheGivenName()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var aCategory = BuildEntity();
        aCategory.Id = 10;

        var anotherCategory = BuildEntity();
        anotherCategory.Id = 20;

        var mockCategoryRepository = MockEntityRepository(aCategory);
        var mockServiceProvider = MockServiceProvider(mockCategoryRepository.Object);

        var context = new ValidationContext(anotherCategory, mockServiceProvider.Object, null);

        // Act
        var result = attribute.GetValidationResult("test_name", context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("The category name 'test_name' is already in use.", result?.ErrorMessage);
    }
}
