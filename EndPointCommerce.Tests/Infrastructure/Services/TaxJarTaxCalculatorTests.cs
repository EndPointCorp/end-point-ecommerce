using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Services;
using EndPointCommerce.Tests.Fixtures;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Tests.Infrastructure.Services;

public class TaxJarTaxCalculatorTests : TransactionalTests
{
    private IConfiguration Configuration { get; set; }

    public TaxJarTaxCalculatorTests(DatabaseFixture database) : base(database)
    {
        Configuration = ConfigurationLoader.LoadConfiguration();
    }

    [Fact]
    public async Task CalculateTest()
    {
        var quote = new Quote()
        {
            ShippingAddress = new Address()
            {
                Name = "John",
                LastName = "Doe",
                ZipCode = "55304",
                CountryId = 1,
                Country = new Country() { Name = "United States", Code = "US" },
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

        var subject = new TaxJarTaxCalculator(Configuration);
        var result = await subject.Calculate(quote);

        Assert.NotNull(result);
        Assert.Equal(1.22M, result.AmountToCollect);
    }
}