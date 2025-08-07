using EndPointEcommerce.AdminPortal.Services;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Tests.Fixtures;

namespace EndPointEcommerce.Tests.AdminPortal.Services;

public class QuoteSearcherTests : BaseEntitySearcherTests<QuoteSearchResultItem, Quote>
{
    public QuoteSearcherTests(DatabaseFixture database) : base(database) { }

    protected override BaseEntitySearcher<QuoteSearchResultItem, Quote> CreateSubject() =>
        new QuoteSearcher(dbContext);

    protected override void PopulateRecords()
    {
        dbContext.Quotes.Add(new Quote
        {
            Id = 10001, IsOpen = true, DateCreated = DateTime.UtcNow.AddDays(-5),
            Customer = new() { Name = "test_name_01", Email = "test_01@email.com" },
            ShippingAddress = new()
            {
                Name = "test_name_01", LastName = "test_last_name_01",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First(s => s.Abbreviation == "AL").Id
            }
        });
        dbContext.Quotes.Add(new Quote
        {
            Id = 10002, IsOpen = true, DateCreated = DateTime.UtcNow.AddDays(-4),
            Customer = new() { Name = "test_name_02", Email = "test_02@email.com" },
            ShippingAddress = new()
            {
                Name = "test_name_02", LastName = "test_last_name_02",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First(s => s.Abbreviation == "CA").Id
            }
        });
        dbContext.Quotes.Add(new Quote
        {
            Id = 10003, IsOpen = true, DateCreated = DateTime.UtcNow.AddDays(-3),
            Customer = new() { Name = "test_name_03", Email = "test_03@email.com" },
            ShippingAddress = new()
            {
                Name = "test_name_03", LastName = "test_last_name_03",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First(s => s.Abbreviation == "DE").Id
            }
        });
        dbContext.Quotes.Add(new Quote
        {
            Id = 10004, IsOpen = true, DateCreated = DateTime.UtcNow.AddDays(-2),
            Customer = new() { Name = "test_name_04", Email = "test_04@email.com" },
            ShippingAddress = new()
            {
                Name = "test_name_04", LastName = "test_last_name_04",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First(s => s.Abbreviation == "FL").Id
            }
        });
        dbContext.Quotes.Add(new Quote
        {
            Id = 10005, IsOpen = true, DateCreated = DateTime.UtcNow.AddDays(-1),
            Customer = new() { Name = "test_name_05", Email = "test_05@email.com" },
            ShippingAddress = new()
            {
                Name = "test_name_05", LastName = "test_last_name_05",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First(s => s.Abbreviation == "GA").Id
            }
        });

        dbContext.SaveChanges();
    }

    [Theory]
    [InlineData("10001", 10001)]
    [InlineData("test_02@email.com", 10002)]
    [InlineData("Georgia", 10005)]
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
    [InlineData("id", "asc", 10001, 10005)]
    [InlineData("id", "desc", 10005, 10001)]
    [InlineData("dateCreated", "asc", 10001, 10005)]
    [InlineData("dateCreated", "desc", 10005, 10001)]
    [InlineData("email", "asc", 10001, 10005)]
    [InlineData("email", "desc", 10005, 10001)]
    [InlineData("shippingAddressStateName", "asc", 10001, 10005)]
    [InlineData("shippingAddressStateName", "desc", 10005, 10001)]
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
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
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
        Assert.Equal(10005, result.Data.First().Id);
        Assert.True((DateTime.UtcNow.AddDays(-5) - DateTime.Parse(result.Data.First().DateCreated!)).TotalSeconds < 2);
        Assert.Equal("test_05@email.com", result.Data.First().Email);
        Assert.Equal(true, result.Data.First().IsOpen);
        Assert.Equal("Georgia", result.Data.First().ShippingAddressStateName);
        Assert.Equal("$0.00", result.Data.First().Total);
        Assert.Equal("test_edit_url", result.Data.First().DetailsUrl);
    }
}
