// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Net;
using System.Net.Http.Json;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Tests.Fixtures;
using EndPointEcommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using static EndPointEcommerce.Domain.Interfaces.IPaymentGateway;

namespace EndPointEcommerce.Tests.WebApi.Controllers;

public class OrdersControllerTests : IntegrationTests
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

        dbContext.Products.Add(newProduct);

        dbContext.SaveChanges();

        return newProduct;
    }

    private State CreateNewState()
    {
        var newState = new State() {
            Name = "test_state",
            Abbreviation = "TS"
        };

        dbContext.States.Add(newState);

        dbContext.SaveChanges();

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
            CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
            StateId = state.Id,
            ZipCode = "12345",
            PhoneNumber = "1234567890"
        };

        dbContext.Addresses.Add(newAddress);

        dbContext.SaveChanges();

        return newAddress;
    }

    private Coupon CreateNewCoupon()
    {
        var newCoupon = new Coupon() {
            Code = "test_coupon_code",
            Discount = 5.00M,
            IsDiscountFixed = true
        };

        dbContext.Coupons.Add(newCoupon);

        dbContext.SaveChanges();

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
                ProductId = dbContext.Products.First().Id,
                Quantity = 1
            }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Set-Cookie", response.Headers.First().Key);
        Assert.Contains("EndPointEcommerce_QuoteId=", response.Headers.First().Value.First());

        response = await client.PutAsJsonAsync(
            "/api/Quote",
            new
            {
                Email = orderEmail,

                CouponCode = dbContext.Coupons.First().Code,

                ShippingAddressId = useShippingAddressId ? dbContext.Addresses.First().Id : (int?)null,
                ShippingAddress = useShippingAddress ? new
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "test_street_one",
                    StreetTwo = "test_street_two",
                    City = "test_city",
                    CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                    StateId = dbContext.States.First().Id,
                    ZipCode = "12345",
                    PhoneNumber = "1234567890",
                } : (object?)null,

                BillingAddressId = useBillingAddressId ? dbContext.Addresses.First().Id : (int?)null,
                BillingAddress = useBillingAddress ? new
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "test_street_one",
                    StreetTwo = "test_street_two",
                    City = "test_city",
                    CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                    StateId = dbContext.States.First().Id,
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

        // Simulate email confirmation
        var user = dbContext.Users.First(u => u.Email == "test@email.com");
        user.EmailConfirmed = true;
        dbContext.SaveChanges();

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

        Assert.Single(dbContext.Quotes);

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
        // Arrange
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
    }

    [Fact]
    public async Task PostOrder_ReturnsNotFound_WhenTheQuoteIsNotOpen()
    {
        var response = await ArrangeAndActPostOrder(async client =>
        {
            // Arrange
            dbContext.Quotes.First().Close();
            dbContext.SaveChanges();

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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoItems()
    {
        var response = await ArrangeAndActPostOrder(async client =>
        {
            // Arrange
            dbContext.Quotes.First().Items.Clear();
            dbContext.SaveChanges();

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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoEmail_AndTheQuoteBelongsToAGuest()
    {
        var response = await ArrangeAndActPostOrder(async client =>
        {
            // Arrange
            dbContext.Quotes.First().Email = null;
            dbContext.SaveChanges();

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
    }

    [Fact]
    public async Task PostOrder_ReturnsOk_WhenTheQuoteHasNoEmail_AndTheQuoteBelongsToACustomer()
    {
        var client = CreateHttpClient();

        await SignUp(client);

        await Login(client);

        // Act
        var response = await ArrangeAndActPostOrder(
            async client =>
            {
                // Arrange
                dbContext.Quotes.First().Email = null;
                dbContext.SaveChanges();

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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoShippingAddress()
    {
        var response = await ArrangeAndActPostOrder(async client => {
            // Arrange
            dbContext.Quotes.First().ShippingAddress = null;
            dbContext.SaveChanges();

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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenTheQuoteHasNoBillingAddress()
    {
        var response = await ArrangeAndActPostOrder(async client => {
            // Arrange
            dbContext.Quotes.First().BillingAddress = null;
            dbContext.SaveChanges();

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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenNoPaymentMethodNonceValueIsGiven()
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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenNoPaymentMethodNonceDescriptorIsGiven()
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
    }

    [Fact]
    public async Task PostOrder_ReturnsBadRequest_WhenThereIsAnErrorCallingThePaymentGateway()
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
    }

    [Fact]
    public async Task PostOrder_CreatesAnOrderBasedOnTheCurrentQuote()
    {
        // Act
        var response = await ArrangeAndActPostOrder();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Single(dbContext.Quotes.ToList());
        Assert.Single(dbContext.QuoteItems.ToList());
        Assert.Single(dbContext.Orders.ToList());
        Assert.Single(dbContext.OrderItems.ToList());

        Assert.Equal("test_customer@email.com", dbContext.Orders.First().Customer.Email);

        Assert.Equal(OrderStatus.PENDING, dbContext.Orders.First().Status.Name);
        Assert.Equal(PaymentMethod.CREDIT_CARD, dbContext.Orders.First().PaymentMethod.Name);
        Assert.Equal("test_payment_method_nonce_value", dbContext.Orders.First().PaymentMethodNonceValue);
        Assert.Equal("test_payment_method_nonce_descriptor", dbContext.Orders.First().PaymentMethodNonceDescriptor);
        Assert.Equal("test_payment_transaction_id", dbContext.Orders.First().PaymentTransactionId);

        var newProduct = dbContext.Products.First();
        var newState = dbContext.States.First();

        Assert.Equal("test_name", dbContext.Orders.First().BillingAddress!.Name);
        Assert.Equal("test_last_name", dbContext.Orders.First().BillingAddress!.LastName);
        Assert.Equal("test_street_one", dbContext.Orders.First().BillingAddress!.Street);
        Assert.Equal("test_street_two", dbContext.Orders.First().BillingAddress!.StreetTwo);
        Assert.Equal("test_city", dbContext.Orders.First().BillingAddress!.City);
        Assert.Equal(newState.Id, dbContext.Orders.First().BillingAddress!.StateId);
        Assert.Equal("12345", dbContext.Orders.First().BillingAddress!.ZipCode);
        Assert.Equal("1234567890", dbContext.Orders.First().BillingAddress!.PhoneNumber);

        Assert.Equal(dbContext.Quotes.First().Id, dbContext.Orders.First().QuoteId);
        Assert.Equal(newProduct.Id, dbContext.OrderItems.First().ProductId);
        Assert.Equal(1, dbContext.OrderItems.First().Quantity);

        Assert.NotNull(dbContext.Orders.First().CouponId);

        Assert.Equal(
            dbContext.Quotes.First().CouponId,
            dbContext.Orders.First().CouponId
        );
    }

    [Fact]
    public async Task PostOrder_AssociatesTheOrderWithTheCurrentLoggedInCustomer_IfTHereIsOne()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);

        await Login(client);

        // Act
        var response = await ArrangeAndActPostOrder(httpClient: client, orderEmail: null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Single(dbContext.Quotes.ToList());
        Assert.Single(dbContext.QuoteItems.ToList());
        Assert.Single(dbContext.Orders.ToList());
        Assert.Single(dbContext.OrderItems.ToList());
        Assert.Single(dbContext.Customers.ToList());

        Assert.Equal("test@email.com", dbContext.Orders.First().Customer.Email);
        Assert.Equal("test@email.com", dbContext.Customers.First().Email);
        Assert.Equal(dbContext.Orders.First().CustomerId, dbContext.Customers.First().Id);
    }

    [Fact]
    public async Task PostOrder_ClosesTheCart()
    {
        // Act
        var response = await ArrangeAndActPostOrder();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.False(dbContext.Quotes.First().IsOpen);
    }

    [Fact]
    public async Task PostOrder_DeletesTheCookie()
    {
        // Act
        var response = await ArrangeAndActPostOrder();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("Set-Cookie", response.Headers.First().Key);
        Assert.Contains("EndPointEcommerce_QuoteId=;", response.Headers.First().Value.First());
        Assert.Contains("expires=Thu, 01 Jan 1970 00:00:00 GMT;", response.Headers.First().Value.First());
    }
}
