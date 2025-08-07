using EndPointEcommerce.AdminPortal.Services;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Tests.Fixtures;

namespace EndPointEcommerce.Tests.AdminPortal.Services;

public class CouponSearcherTests : BaseEntitySearcherTests<CouponSearchResultItem, Coupon>
{
    public CouponSearcherTests(DatabaseFixture database) : base(database) { }

    protected override BaseEntitySearcher<CouponSearchResultItem, Coupon> CreateSubject() =>
        new CouponSearcher(dbContext);

    protected override void PopulateRecords()
    {
        dbContext.Coupons.Add(new Coupon
        {
            Id = 1, Code = "test_code_01", Discount = 10, IsDiscountFixed = true,
            DateCreated = DateTime.UtcNow.AddDays(-5), DateModified = DateTime.UtcNow.AddDays(-5)
        });
        dbContext.Coupons.Add(new Coupon
        {
            Id = 2, Code = "test_code_02", Discount = 20, IsDiscountFixed = false,
            DateCreated = DateTime.UtcNow.AddDays(-4), DateModified = DateTime.UtcNow.AddDays(-4)
        });
        dbContext.Coupons.Add(new Coupon
        {
            Id = 3, Code = "test_code_03", Discount = 30, IsDiscountFixed = true,
            DateCreated = DateTime.UtcNow.AddDays(-3), DateModified = DateTime.UtcNow.AddDays(-3)
        });
        dbContext.Coupons.Add(new Coupon
        {
            Id = 4, Code = "test_code_04", Discount = 40, IsDiscountFixed = false,
            DateCreated = DateTime.UtcNow.AddDays(-2), DateModified = DateTime.UtcNow.AddDays(-2)
        });
        dbContext.Coupons.Add(new Coupon
        {
            Id = 5, Code = "test_code_05", Discount = 50, IsDiscountFixed = true,
            DateCreated = DateTime.UtcNow.AddDays(-1), DateModified = DateTime.UtcNow.AddDays(-1)
        });

        dbContext.SaveChanges();
    }

    [Theory]
    [InlineData("test_code_01", 1)]
    [InlineData("test_code_02", 2)]
    public async Task Search_CanSearch(string searchValue, int matchedItemId)
    {
        await RunSearchingTheory(
            searchValue,
            result => {
                Assert.Equal(matchedItemId, result.Data.First().Id);
            }
        );
    }

    [Theory]
    [InlineData("code", "asc", 1, 5)]
    [InlineData("code", "desc", 5, 1)]
    [InlineData("discount", "asc", 1, 5)]
    [InlineData("discount", "desc", 5, 1)]
    [InlineData("dateCreated", "asc", 1, 5)]
    [InlineData("dateCreated", "desc", 5, 1)]
    [InlineData("dateModified", "asc", 1, 5)]
    [InlineData("dateModified", "desc", 5, 1)]
    public async Task Search_CanSort(string orderByName, string orderByDirection, int firstItemId, int lastItemId)
    {
        await RunSortingTheory(
            orderByName, orderByDirection,
            result => {
                Assert.Equal(firstItemId, result.Data.First().Id);
                Assert.Equal(lastItemId, result.Data.Last().Id);
            }
        );
    }

    [Fact]
    public async Task Search_ReturnsCorrectlyPopulatedData()
    {
        // Arrange
        PopulateRecords();

        var parameters = new SearchParameters
        {
            Draw = 1,
            Start = 0,
            Length = 100,
            Search = null,
            Order = null,
        };

        // Act
        var result = await _subject.Search(parameters, _mockUrlBuilder.Object);

        // Assert
        Assert.Equal(1, result.Draw);
        Assert.Equal(5, result.Data.Count);
        Assert.Equal(1, result.Data.First().Id);
        Assert.Equal("test_code_01", result.Data.First().Code);
        Assert.Equal(10, result.Data.First().Discount);
        Assert.Equal(true, result.Data.First().IsDiscountFixed);
        Assert.True((DateTime.UtcNow.AddDays(-5) - DateTime.Parse(result.Data.First().DateCreated!)).TotalSeconds < 2);
        Assert.True((DateTime.UtcNow.AddDays(-5) - DateTime.Parse(result.Data.First().DateModified!)).TotalSeconds < 2);
        Assert.Equal("test_edit_url", result.Data.First().EditUrl);
    }
}
