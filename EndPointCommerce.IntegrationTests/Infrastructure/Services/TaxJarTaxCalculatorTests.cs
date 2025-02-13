using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Services;
using EndPointCommerce.IntegrationTests.Fixtures;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.IntegrationTests.Infrastructure.Services;

public class TaxJarTaxCalculatorTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _database;
    private readonly EndPointCommerceDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public TaxJarTaxCalculatorTests(DatabaseFixture database)
    {
        _database = database;
        _dbContext = database.CreateDbContext();
        _configuration = GetConfiguration();
    }

    public static IConfiguration GetConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        return config;
    }

    [Fact(Skip = "Test case is disabled for now.")]
    public async Task CalculateTest()
    {
        await _database.WithTransaction(_dbContext, async () =>
        {
            var quote = new Quote()
            {
                ShippingAddress = new Address()
                {
                    Name = "John",
                    LastName = "Doe",
                    ZipCode = "55304",
                    StateId = 0,
                    State = new State() { Name = "Minnesota", Abbreviation = "MN" },
                    City = "Ham Lake",
                    Street = "123 Main St"
                }
            };

            quote.Items =
            [
                new()
                {
                    Id = 10,
                    Quote = quote,
                    Quantity = 1,

                    Product = new Product()
                    {
                        Id = 1,
                        Name = "Test Product",
                        Sku = "test_product",
                        BasePrice = 15.0M
                    }
                }
            ];

            var subject = new TaxJarTaxCalculator(_configuration);
            var result = await subject.Calculate(quote);

            Assert.NotNull(result);
            Assert.Equal(1.22M, result.AmountToCollect);
        });
    }
}