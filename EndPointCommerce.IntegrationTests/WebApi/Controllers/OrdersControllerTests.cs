using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.IntegrationTests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using static EndPointCommerce.Domain.Interfaces.IPaymentGateway;

namespace EndPointCommerce.IntegrationTests.WebApi.Controllers;

public class OrdersControllerTests : IntegrationTestFixture
{
    public OrdersControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private Product CreateNewProduct()
    {
        var newProduct = new Product() {
            Name = "test_product",
            Sku = "test_sku",
            BasePrice = 10.00M
        };

        _dbContext.Products.Add(newProduct);

        _dbContext.SaveChanges();

        return newProduct;
    }

    private State CreateNewState()
    {
        var newState = new State() {
            Name = "test_state",
            Abbreviation = "TS"
        };

        _dbContext.States.Add(newState);

        _dbContext.SaveChanges();

        return newState;
    }

    private Address CreateNewAddress(State state)
    {
        var newAddress = new Address()
        {
            Name = "test_name",
            LastName = "test_last_name",
            Street = "test_street_one",
            StreetTwo = "test_street_two",
            City = "test_city",
            CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
            StateId = state.Id,
            ZipCode = "12345",
            PhoneNumber = "1234567890"
        };

        _dbContext.Addresses.Add(newAddress);

        _dbContext.SaveChanges();

        return newAddress;
    }

    private Coupon CreateNewCoupon()
    {
        var newCoupon = new Coupon() {
            Code = "test_coupon_code",
            Discount = 5.00M,
            IsDiscountFixed = true
        };

        _dbContext.Coupons.Add(newCoupon);

        _dbContext.SaveChanges();

        return newCoupon;
    }

    private async Task<HttpResponseMessage> CreateAndValidateQuote(
        HttpClient client,
        string? orderEmail = "test_customer@email.com",
        bool useShippingAddressId = true,
        bool useShippingAddress = true,
        bool useBillingAddressId = true,
        bool useBillingAddress = true
    ) {
        CreateNewProduct();
        CreateNewAddress(CreateNewState());
        CreateNewCoupon();

        var response = await client.PostAsJsonAsync(
            "/api/Quote/Items",
            new
            {
                ProductId = _dbContext.Products.First().Id,
                Quantity = 1
            }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Set-Cookie", response.Headers.First().Key);
        Assert.Contains("EndPointCommerce_QuoteId=", response.Headers.First().Value.First());

        response = await client.PutAsJsonAsync(
            "/api/Quote",
            new
            {
                Email = orderEmail,

                CouponCode = _dbContext.Coupons.First().Code,

                ShippingAddressId = useShippingAddressId ? _dbContext.Addresses.First().Id : (int?)null,
                ShippingAddress = useShippingAddress ? new
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "test_street_one",
                    StreetTwo = "test_street_two",
                    City = "test_city",
                    CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                    StateId = _dbContext.States.First().Id,
                    ZipCode = "12345",
                    PhoneNumber = "1234567890",
                } : (object?)null,

                BillingAddressId = useBillingAddressId ? _dbContext.Addresses.First().Id : (int?)null,
                BillingAddress = useBillingAddress ? new
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "test_street_one",
                    StreetTwo = "test_street_two",
                    City = "test_city",
                    CountryId = _dbContext.Countries.First(c => c.Code == "US").Id,
                    StateId = _dbContext.States.First().Id,
                    ZipCode = "12345",
                    PhoneNumber = "1234567890",
                } : (object?)null
            }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await client.PostAsJsonAsync("api/Quote/Validate", new { });

        return response;
    }

    private async Task<HttpResponseMessage> SignUp(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/User",
            new
            {
                Email = "test@email.com",
                Password = "TEST_password_123",
                Name = "test_name",
                LastName = "test_last_name"
            }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return response;
    }

    private async Task<HttpResponseMessage> Login(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/User/login?useCookies=true&useSessionCookies=false",
            new
            {
                Email = "test@email.com",
                Password = "TEST_password_123",
            }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Set-Cookie", response.Headers.First().Key);
        Assert.Contains(".AspNetCore.Identity.Application", response.Headers.First().Value.First());

        return response;
    }

    private async Task<HttpResponseMessage> ArrangeAndActPostOrder(
        Func<HttpClient, Task<HttpResponseMessage>>? postOrder = null,
        HttpClient? httpClient = null,
        string? orderEmail = "test_customer@email.com"
    ) {
        var client = httpClient ?? CreateHttpClient();

        var response = await CreateAndValidateQuote(client, orderEmail: orderEmail);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Single(_dbContext.Quotes);

        // Act
        if (postOrder == null)
        {
            response = await client.PostAsJsonAsync(
                "/api/Orders",
                new
                {
                    PaymentMethodNonceValue = "test_payment_method_nonce_value",
                    PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                }
            );
        }
        else
        {
            response = await postOrder.Invoke(client);
        }

        return response;
    }

    [Fact]
    public async Task PostOrder_ReturnsNotFound_WhenNoQuoteHasBeenCreated_BecauseThereIsNoCookie()
    {
        await WithTransaction(async () => {
            // Arrange
            var state = _dbContext.States.First();

            var client = CreateHttpClient();

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Orders",
                new
                {
                    PaymentMethodNonceValue = "test_payment_method_nonce_value",
                    PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsNotFound_WhenTheQuoteIsNotOpen()
    {
        await WithTransaction(async () =>
        {
            var response = await ArrangeAndActPostOrder(async client =>
            {
                // Arrange
                _dbContext.Quotes.First().Close();
                _dbContext.SaveChanges();

                // Act
                return await client.PostAsJsonAsync(
                    "/api/Orders",
                    new
                    {
                        PaymentMethodNonceValue = "test_payment_method_nonce_value",
                        PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                    }
                );
            });

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoItems()
    {
        await WithTransaction(async () =>
        {
            var response = await ArrangeAndActPostOrder(async client =>
            {
                // Arrange
                _dbContext.Quotes.First().Items.Clear();
                _dbContext.SaveChanges();

                // Act
                return await client.PostAsJsonAsync(
                    "/api/Orders",
                    new
                    {
                        PaymentMethodNonceValue = "test_payment_method_nonce_value",
                        PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                    }
                );
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoEmail_AndTheQuoteBelongsToAGuest()
    {
        await WithTransaction(async () =>
        {
            var response = await ArrangeAndActPostOrder(async client =>
            {
                // Arrange
                _dbContext.Quotes.First().Email = null;
                _dbContext.SaveChanges();

                // Act
                return await client.PostAsJsonAsync(
                    "/api/Orders",
                    new
                    {
                        PaymentMethodNonceValue = "test_payment_method_nonce_value",
                        PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                    }
                );
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsOk_WhenTheQuoteHasNoEmail_AndTheQuoteBelongsToACustomer()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            // Act
            var response = await ArrangeAndActPostOrder(
                async client =>
                {
                    // Arrange
                    _dbContext.Quotes.First().Email = null;
                    _dbContext.SaveChanges();

                    // Act
                    return await client.PostAsJsonAsync(
                        "/api/Orders",
                        new
                        {
                            PaymentMethodNonceValue = "test_payment_method_nonce_value",
                            PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                        }
                    );
                },
                httpClient: client
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoShippingAddress()
    {
        await WithTransaction(async () => {
            var response = await ArrangeAndActPostOrder(async client => {
                // Arrange
                _dbContext.Quotes.First().ShippingAddress = null;
                _dbContext.SaveChanges();

                // Act
                return await client.PostAsJsonAsync(
                    "/api/Orders",
                    new
                    {
                        PaymentMethodNonceValue = "test_payment_method_nonce_value",
                        PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                    }
                );
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoBillingAddress()
    {
        await WithTransaction(async () => {
            var response = await ArrangeAndActPostOrder(async client => {
                // Arrange
                _dbContext.Quotes.First().BillingAddress = null;
                _dbContext.SaveChanges();

                // Act
                return await client.PostAsJsonAsync(
                    "/api/Orders",
                    new
                    {
                        PaymentMethodNonceValue = "test_payment_method_nonce_value",
                        PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                    }
                );
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenNoPaymentMethodNonceValueIsGiven()
    {
        await WithTransaction(async () =>
        {
            // Arrange
            var client = CreateHttpClient();

            var response = await CreateAndValidateQuote(client);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Orders",
                new
                {
                    PaymentMethodNonceValue = (string?)null,
                    PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();

            Assert.NotNull(result);
            Assert.Equal("The PaymentMethodNonceValue is required for non-free orders", result["Message"]);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenNoPaymentMethodNonceDescriptorIsGiven()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            var response = await CreateAndValidateQuote(client, "test_customer@email.com");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Orders",
                new
                {
                    PaymentMethodNonceValue = "test_payment_method_nonce_value",
                    PaymentMethodNonceDescriptor = (string?)null,
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();

            Assert.NotNull(result);
            Assert.Equal("The PaymentMethodNonceDescriptor is required for non-free orders", result["Message"]);
        });
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenThereIsAnErrorCallingThePaymentGateway()
    {
        await WithTransaction(async () =>
        {
            // Arrange
            var mockPaymentGateway = new Mock<IPaymentGateway>();

            mockPaymentGateway
                .Setup(m => m.CreatePaymentTransaction(It.IsAny<Order>()))
                .Returns(new PaymentTransactionResult() { IsSuccess = false });

            var client = CreateHttpClient(mockPaymentGateway: mockPaymentGateway);

            var response = await CreateAndValidateQuote(client);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Orders",
                new
                {
                    PaymentMethodNonceValue = "test_payment_method_nonce_value",
                    PaymentMethodNonceDescriptor = "test_payment_method_nonce_descriptor"
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();

            Assert.NotNull(result);
            Assert.Equal("Error processing payment", result["Message"]);
        });
    }

    [Fact]
    public async Task PostOrder_CreatesAnOrderBasedOnTheCurrentQuote()
    {
        await WithTransaction(async () => {
            // Act
            var response = await ArrangeAndActPostOrder();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Single(_dbContext.Orders.ToList());
            Assert.Single(_dbContext.OrderItems.ToList());

            Assert.Equal("test_customer@email.com", _dbContext.Orders.First().Customer.Email);

            Assert.Equal(OrderStatus.PENDING, _dbContext.Orders.First().Status.Name);
            Assert.Equal(PaymentMethod.CREDIT_CARD, _dbContext.Orders.First().PaymentMethod.Name);
            Assert.Equal("test_payment_method_nonce_value", _dbContext.Orders.First().PaymentMethodNonceValue);
            Assert.Equal("test_payment_method_nonce_descriptor", _dbContext.Orders.First().PaymentMethodNonceDescriptor);
            Assert.Equal("test_payment_transaction_id", _dbContext.Orders.First().PaymentTransactionId);

            var newProduct = _dbContext.Products.First();
            var newState = _dbContext.States.First();

            Assert.Equal("test_name", _dbContext.Orders.First().BillingAddress!.Name);
            Assert.Equal("test_last_name", _dbContext.Orders.First().BillingAddress!.LastName);
            Assert.Equal("test_street_one", _dbContext.Orders.First().BillingAddress!.Street);
            Assert.Equal("test_street_two", _dbContext.Orders.First().BillingAddress!.StreetTwo);
            Assert.Equal("test_city", _dbContext.Orders.First().BillingAddress!.City);
            Assert.Equal(newState.Id, _dbContext.Orders.First().BillingAddress!.StateId);
            Assert.Equal("12345", _dbContext.Orders.First().BillingAddress!.ZipCode);
            Assert.Equal("1234567890", _dbContext.Orders.First().BillingAddress!.PhoneNumber);

            Assert.Equal(_dbContext.Quotes.First().Id, _dbContext.Orders.First().QuoteId);
            Assert.Equal(newProduct.Id, _dbContext.OrderItems.First().ProductId);
            Assert.Equal(1, _dbContext.OrderItems.First().Quantity);

            Assert.NotNull(_dbContext.Orders.First().CouponId);

            Assert.Equal(
                _dbContext.Quotes.First().CouponId,
                _dbContext.Orders.First().CouponId
            );
        });
    }

    [Fact]
    public async Task PostOrder_AssociatesTheOrderWithTheCurrentLoggedInCustomer_IfTHereIsOne()
    {
        await WithTransaction(async () => {
            // Arrange
            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            // Act
            var response = await ArrangeAndActPostOrder(httpClient: client, orderEmail: null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Single(_dbContext.Orders.ToList());
            Assert.Single(_dbContext.OrderItems.ToList());
            Assert.Single(_dbContext.Customers.ToList());

            Assert.Equal("test@email.com", _dbContext.Orders.First().Customer.Email);
            Assert.Equal("test@email.com", _dbContext.Customers.First().Email);
            Assert.Equal(_dbContext.Orders.First().CustomerId, _dbContext.Customers.First().Id);
        });
    }

    [Fact]
    public async Task PostOrder_ClosesTheCart()
    {
        await WithTransaction(async () => {
            // Act
            var response = await ArrangeAndActPostOrder();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.False(_dbContext.Quotes.First().IsOpen);
        });
    }

    [Fact]
    public async Task PostOrder_DeletesTheCookie()
    {
        await WithTransaction(async () =>
        {
            // Act
            var response = await ArrangeAndActPostOrder();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("Set-Cookie", response.Headers.First().Key);
            Assert.Contains("EndPointCommerce_QuoteId=;", response.Headers.First().Value.First());
            Assert.Contains("expires=Thu, 01 Jan 1970 00:00:00 GMT;", response.Headers.First().Value.First());
        });
    }
}
