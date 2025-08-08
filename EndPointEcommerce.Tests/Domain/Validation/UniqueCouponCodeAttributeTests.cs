using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Validation;
using Moq;

namespace EndPointEcommerce.Tests.Domain.Validation;

public class UniqueCouponCodeAttributeTests : BaseUniqueFieldAttributeTests<Coupon, ICouponRepository>
{
    protected override UniqueCouponCodeAttribute BuildTestSubject() => new();

    protected override Coupon BuildEntity() =>
        new() { Code = "test_code", Discount = 0, IsDiscountFixed = true };

    protected override Mock<ICouponRepository> MockEntityRepository(object? returnValue)
    {
        var mockCategoryRepository = new Mock<ICouponRepository>();

        mockCategoryRepository
            .Setup(m => m.FindByCodeAsync("test_code"))
            .ReturnsAsync((Coupon?)returnValue);

        return mockCategoryRepository;
    }

    [Fact]
    public void GetValidationResult_ReturnsFailure_WhenAnotherCouponExistsWithTheGivenName()
    {
        // Arrange
        var attribute = BuildTestSubject();

        var aCoupon = BuildEntity();
        aCoupon.Id = 10;

        var anotherCoupon = new Coupon { Id = 20, Code = "test_code", Discount = 0, IsDiscountFixed = true };

        var mockCategoryRepository = MockEntityRepository(aCoupon);
        var mockServiceProvider = MockServiceProvider(mockCategoryRepository.Object);

        var context = new ValidationContext(anotherCoupon, mockServiceProvider.Object, null);

        // Act
        var result = attribute.GetValidationResult("test_code", context);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("The coupon code 'test_code' is already in use.", result?.ErrorMessage);
    }
}
