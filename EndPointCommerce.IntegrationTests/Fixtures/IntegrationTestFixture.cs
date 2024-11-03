using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using EndPointCommerce.Infrastructure.Services;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using static EndPointCommerce.Domain.Interfaces.IPaymentGateway;
using static EndPointCommerce.Domain.Interfaces.ITaxCalculator;

namespace EndPointCommerce.IntegrationTests.Fixtures;

public abstract class IntegrationTestFixture : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly DatabaseFixture _database;
    protected readonly EndPointCommerceDbContext _dbContext;

    public IntegrationTestFixture(WebApplicationFactory<Program> factory, DatabaseFixture database)
    {
        _factory = factory;
        _database = database;

        _dbContext = database.CreateDbContext();
    }

    protected HttpClient CreateHttpClient(
        Mock<IPaymentGateway>? mockPaymentGateway = null,
        Mock<ITaxCalculator>? mockTaxCalculator = null
    ) {
        // Disable emails for integration tests
        var mockMailer = new Mock<IMailer>();
        mockMailer.Setup(m => m.SendMailAsync(It.IsAny<MailData>()));

        // Disable interactions with Authorize.NET for integration tests
        if (mockPaymentGateway == null)
        {
            mockPaymentGateway = new Mock<IPaymentGateway>();
            mockPaymentGateway
                .Setup(m => m.CreatePaymentTransaction(It.IsAny<Order>()))
                .Returns(new PaymentTransactionResult()
                {
                    IsSuccess = true,
                    TransactionId = "test_payment_transaction_id"
                });
        }

        // Disable interactions with TaxJar for integration tests
        if (mockTaxCalculator == null)
        {
            mockTaxCalculator = new Mock<ITaxCalculator>();
            mockTaxCalculator
                .Setup(m => m.Calculate(It.IsAny<Quote>()))
                .ReturnsAsync(new TaxResponse()
                {
                    AmountToCollect = 12.34M
                });
        }

        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(_ => _dbContext);
                services.AddTransient(_ => mockMailer.Object);
                services.AddTransient(_ => mockPaymentGateway.Object);
                services.AddTransient(_ => mockTaxCalculator.Object);
            });
        })
        .CreateClient();
    }

    protected async Task WithTransaction(Func<Task> test)
    {
        await _database.WithTransaction(_dbContext, test);
    }
}