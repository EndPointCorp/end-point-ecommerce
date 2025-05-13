using EndPointCommerce.AdminPortal.Services;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.IntegrationTests.Fixtures;

namespace EndPointCommerce.IntegrationTests.AdminPortal.Services;

public class OrderSearcherTests : BaseEntitySearcherTests<OrderSearchResultItem, Order>
{
    public OrderSearcherTests(DatabaseFixture database) : base(database) { }

    protected override BaseEntitySearcher<OrderSearchResultItem, Order> CreateSubject() =>
        new OrderSearcher(_dbContext);

    protected override void PopulateRecords()
    {
        _dbContext.Orders.Add(new Order
        {
            Id = 10001,
            DateCreated = DateTime.UtcNow.AddDays(-5),
            Customer = new() { Name = "test_name_01", Email = "test_01@email.com" },
            Status = _dbContext.OrderStatuses.First(os => os.Name == OrderStatus.PENDING),
            PaymentMethod = _dbContext.PaymentMethods.First(os => os.Name == PaymentMethod.CREDIT_CARD),
            ShippingAddress = new()
            {
                Name = "test_name_01", LastName = "test_last_name_01",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "AL").Id
            },
            BillingAddress = new()
            {
                Name = "test_name_01", LastName = "test_last_name_01",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "AL").Id
            },
            Total = 100.00M
        });
        _dbContext.Orders.Add(new Order
        {
            Id = 10002,
            DateCreated = DateTime.UtcNow.AddDays(-4),
            Customer = new() { Name = "test_name_02", Email = "test_02@email.com" },
            Status = _dbContext.OrderStatuses.First(os => os.Name == OrderStatus.INVOICED),
            PaymentMethod = _dbContext.PaymentMethods.First(os => os.Name == PaymentMethod.CREDIT_CARD),
            ShippingAddress = new()
            {
                Name = "test_name_02", LastName = "test_last_name_02",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "CA").Id
            },
            BillingAddress = new()
            {
                Name = "test_name_02", LastName = "test_last_name_02",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "CA").Id
            },
            Total = 200.00M
        });
        _dbContext.Orders.Add(new Order
        {
            Id = 10003,
            DateCreated = DateTime.UtcNow.AddDays(-3),
            Customer = new() { Name = "test_name_03", Email = "test_03@email.com" },
            Status = _dbContext.OrderStatuses.First(os => os.Name == OrderStatus.PROCESSING),
            PaymentMethod = _dbContext.PaymentMethods.First(os => os.Name == PaymentMethod.CREDIT_CARD),
            ShippingAddress = new()
            {
                Name = "test_name_03", LastName = "test_last_name_03",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "DE").Id
            },
            BillingAddress = new()
            {
                Name = "test_name_03", LastName = "test_last_name_03",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "DE").Id
            },
            Total = 300.00M
        });
        _dbContext.Orders.Add(new Order
        {
            Id = 10004,
            DateCreated = DateTime.UtcNow.AddDays(-2),
            Customer = new() { Name = "test_name_04", Email = "test_04@email.com" },
            Status = _dbContext.OrderStatuses.First(os => os.Name == OrderStatus.CANCELLED),
            PaymentMethod = _dbContext.PaymentMethods.First(os => os.Name == PaymentMethod.CREDIT_CARD),
            ShippingAddress = new()
            {
                Name = "test_name_04", LastName = "test_last_name_04",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "FL").Id
            },
            BillingAddress = new()
            {
                Name = "test_name_04", LastName = "test_last_name_04",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "FL").Id
            },
            Total = 400.00M
        });
        _dbContext.Orders.Add(new Order
        {
            Id = 10005,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            Customer = new() { Name = "test_name_05", Email = "test_05@email.com" },
            Status = _dbContext.OrderStatuses.First(os => os.Name == OrderStatus.PENDING),
            PaymentMethod = _dbContext.PaymentMethods.First(os => os.Name == PaymentMethod.CREDIT_CARD),
            ShippingAddress = new()
            {
                Name = "test_name_05", LastName = "test_last_name_05",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "GA").Id
            },
            BillingAddress = new()
            {
                Name = "test_name_05", LastName = "test_last_name_05",
                ZipCode = "12345", City = "test_city", Street = "test_street",
                CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = _dbContext.States.First(s => s.Abbreviation == "GA").Id
            },
            Total = 500.00M
        });

        _dbContext.SaveChanges();
    }

    [Theory]
    [InlineData("10001", 10001)]
    [InlineData("test_name_02", 10002)]
    [InlineData("Processing", 10003)]
    [InlineData("500", 10005)]
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
    [InlineData("customerFullName", "asc", 10001, 10005)]
    [InlineData("customerFullName", "desc", 10005, 10001)]
    [InlineData("statusName", "asc", 10004, 10003)]
    [InlineData("statusName", "desc", 10003, 10004)]
    [InlineData("billingAddressStateName", "asc", 10001, 10005)]
    [InlineData("billingAddressStateName", "desc", 10005, 10001)]
    [InlineData("total", "asc", 10001, 10005)]
    [InlineData("total", "desc", 10005, 10001)]
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
        await _database.WithTransaction(_dbContext, async () =>
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
            Assert.True((DateTime.UtcNow.AddDays(-1) - DateTime.Parse(result.Data.First().DateCreated!)).TotalSeconds < 2);
            Assert.Equal("test_name_05", result.Data.First().CustomerFullName);
            Assert.Equal("Pending", result.Data.First().StatusName);
            Assert.Equal("Georgia", result.Data.First().BillingAddressStateName);
            Assert.Equal("$500.00", result.Data.First().Total);
            Assert.Equal("test_edit_url", result.Data.First().EditUrl);
        });
    }
}
