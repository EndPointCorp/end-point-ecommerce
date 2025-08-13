using EndPointEcommerce.AdminPortal.Services;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Tests.Fixtures;

namespace EndPointEcommerce.Tests.AdminPortal.Services;

public class CustomerSearcherTests : BaseEntitySearcherTests<CustomerSearchResultItem, Customer>
{
    public CustomerSearcherTests(DatabaseFixture database) : base(database) { }

    protected override BaseEntitySearcher<CustomerSearchResultItem, Customer> CreateSubject() =>
        new CustomerSearcher(dbContext);

    protected override void PopulateRecords()
    {
        dbContext.Customers.Add(new Customer
        {
            Id = 1, Name = "test_name_01", LastName = "test_last_name_01", Email = "test_01@email.com",
            DateCreated = DateTime.UtcNow.AddDays(-5)
        });
        dbContext.Customers.Add(new Customer
        {
            Id = 2, Name = "test_name_02", LastName = "test_last_name_02", Email = "test_02@email.com",
            DateCreated = DateTime.UtcNow.AddDays(-4)
        });
        dbContext.Customers.Add(new Customer
        {
            Id = 3, Name = "test_name_03", LastName = "test_last_name_03", Email = "test_03@email.com",
            DateCreated = DateTime.UtcNow.AddDays(-3)
        });
        dbContext.Customers.Add(new Customer
        {
            Id = 4, Name = "test_name_04", LastName = "test_last_name_04", Email = "test_04@email.com",
            DateCreated = DateTime.UtcNow.AddDays(-2)
        });
        dbContext.Customers.Add(new Customer
        {
            Id = 5, Name = "test_name_05", LastName = "test_last_name_05", Email = "test_05@email.com",
            DateCreated = DateTime.UtcNow.AddDays(-1)
        });

        dbContext.SaveChanges();
    }

    [Theory]
    [InlineData("test_name_01", 1)]
    [InlineData("test_name_05", 5)]
    [InlineData("test_02@email.com", 2)]
    [InlineData("test_04@email.com", 4)]
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
    [InlineData("name", "asc", 1, 5)]
    [InlineData("name", "desc", 5, 1)]
    [InlineData("lastName", "asc", 1, 5)]
    [InlineData("lastName", "desc", 5, 1)]
    [InlineData("email", "asc", 1, 5)]
    [InlineData("email", "desc", 5, 1)]
    [InlineData("dateCreated", "asc", 1, 5)]
    [InlineData("dateCreated", "desc", 5, 1)]
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
        Assert.Equal("test_name_01", result.Data.First().Name);
        Assert.Equal("test_last_name_01", result.Data.First().LastName);
        Assert.Equal("test_01@email.com", result.Data.First().Email);
        Assert.True((DateTime.UtcNow.AddDays(-5) - DateTime.Parse(result.Data.First().DateCreated!)).TotalSeconds < 2);
        Assert.Equal("test_edit_url", result.Data.First().EditUrl);
    }
}
