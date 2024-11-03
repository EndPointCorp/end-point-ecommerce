using System.ComponentModel.DataAnnotations;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Validation;
using Moq;

namespace EndPointCommerce.UnitTests.Domain.Validation;

public abstract class BaseUniqueFieldAttributeTests<TEntity, TRepo>
    where TEntity : BaseEntity
    where TRepo : class
{
    protected abstract BaseUniqueFieldAttribute<TRepo> BuildTestSubject();
    protected abstract TEntity BuildEntity();
    protected abstract Mock<TRepo> MockEntityRepository(object? entity);

    protected static Mock<IServiceProvider> MockServiceProvider(TRepo? repository)
    {
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider
            .Setup(m => m.GetService(typeof(TRepo)))
            .Returns(repository);

        return mockServiceProvider;
    }

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenTheFieldThatTheAttributeIsAppliedToIsNull()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var context = new ValidationContext(BuildEntity());

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenARepositoryCannotBeFoundThroughDependencyInjection()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var mockServiceProvider = MockServiceProvider(null);

        var context = new ValidationContext(
            BuildEntity(),
            mockServiceProvider.Object,
            null
        );

        // Act
        var result = attribute.GetValidationResult("test_unique_field", context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenNoRecordExistsWithTheGivenUniqueField()
    {
        // Arrange
        var attribute = BuildTestSubject();
        var mockProductRepository = MockEntityRepository(null);
        var mockServiceProvider = MockServiceProvider(mockProductRepository.Object);

        var context = new ValidationContext(BuildEntity(), mockServiceProvider.Object, null);

        // Act
        var result = attribute.GetValidationResult("test_unique_field", context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GetValidationResult_ReturnsSuccess_WhenARecordExistsWithTheGivenUniqueFieldButItIsTheOneBeingCurrentlyValidated()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var anEntity = BuildEntity();
        anEntity.Id = 10;

        var mockCategoryRepository = MockEntityRepository(anEntity);
        var mockServiceProvider = MockServiceProvider(mockCategoryRepository.Object);

        var context = new ValidationContext(anEntity, mockServiceProvider.Object, null);

        // Act
        var result = attribute.GetValidationResult("test_unique_field", context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }
}