using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.UnitTests.Domain.Entities;

public class CouponTests
{
    [Fact]
    public void CodeIs_ShouldReturnTrue_WhenCodesAreEqual()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "test_code_10",
            Discount = 10,
            IsDiscountFixed = true
        };

        // Act
        var result = coupon.CodeIs("test_code_10");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CodeIs_ShouldReturnTrue_WhenCodesAreEqualIgnoringCase()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "test_code_10",
            Discount = 10,
            IsDiscountFixed = true
        };

        // Act
        var result = coupon.CodeIs("TEST_CODE_10");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CodeIs_ShouldReturnFalse_WhenCodesAreNotEqual()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "test_code_10",
            Discount = 10,
            IsDiscountFixed = true
        };

        // Act
        var result = coupon.CodeIs("another_test_code_10");

        // Assert
        Assert.False(result);
    }
}