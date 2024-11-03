using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.IntegrationTests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointCommerce.IntegrationTests.WebApi.Controllers;

public class QuoteControllerTests : IntegrationTestFixture
{
    public QuoteControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private Product CreateNewProduct(string? name = null)
    {
        var newProduct = new Product() {
            Name = name ?? "test_product",
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
            Discount = 10.00M,
            IsDiscountFixed = false
        };

        _dbContext.Coupons.Add(newCoupon);

        _dbContext.SaveChanges();

        return newCoupon;
    }

    private Coupon CreateNewFixedCoupon()
    {
        var newCoupon = new Coupon()
        {
            Code = "fixed_coupon_code",
            Discount = 1.5M,
            IsDiscountFixed = true,
        };

        _dbContext.Coupons.Add(newCoupon);

        _dbContext.SaveChanges();

        return newCoupon;
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

    private async Task<HttpResponseMessage> CreateAndValidateQuote(
        HttpClient client,
        string? orderEmail = "test_email@email.com",
        bool useShippingAddressId = true,
        bool useShippingAddress = true,
        bool useBillingAddressId = true,
        bool useBillingAddress = true
    ) {
        CreateNewProduct();
        CreateNewAddress(CreateNewState());

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

                ShippingAddressId = useShippingAddressId ? _dbContext.Addresses.First().Id : (int?)null,
                ShippingAddress = useShippingAddress ? new
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "test_street_one",
                    StreetTwo = "test_street_two",
                    City = "test_city",
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

    [Fact]
    public async Task GetQuote_RespodsWithTheCurrentQuote_WhenAQuoteHasBeenCreated_ByAPreviousCallToPostQuoteItem()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.GetAsync("/api/Quote");

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(newProduct.Id, quote!.Items.First().ProductId);
            Assert.Equal(3, quote!.Items.First()!.Quantity);
            Assert.Equal(10.00M, quote!.Items.First()!.UnitPrice);
            Assert.Equal(30.00M, quote!.Items.First()!.Total);
        });
    }

    [Fact]
    public async Task GetQuote_ReturnsNotFound_WhenNoQuoteHasBeenCreated_BecauseThereIsNoCookie()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Quote");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetQuote_ReturnsNotFound_WhenNoQuoteHasBeenCreated_BecauseTheLoggedInUserHasNoQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            // Act
            var response = await client.GetAsync("/api/Quote");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task GetQuote_RespodsWithCurrentQuote_OwnedByTheLoggedInUser_WhenAQuoteHasBeenCreated_ByAPreviousCallToPostQuoteItem()
    {
        await WithTransaction(async () => {
            // Arrange
            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            var newProduct = CreateNewProduct();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.GetAsync("/api/Quote");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(newProduct.Id, quote!.Items.First().ProductId);
            Assert.Equal(3, quote!.Items.First()!.Quantity);
            Assert.Equal(10.00M, quote!.Items.First()!.UnitPrice);
            Assert.Equal(30.00M, quote!.Items.First()!.Total);

            Assert.Equal(
                _dbContext.Users.First(u => u.Email == "test@email.com").CustomerId,
                _dbContext.Quotes.First().CustomerId
            );
        });
    }

    [Fact]
    public async Task GetQuote_ReturnsNotFound_WhenTheCurrentQuoteIsClosed()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quote = _dbContext.Quotes.First();
            quote.Close();
            _dbContext.SaveChanges();

            // Act
            response = await client.GetAsync("/api/Quote");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task GetQuote_AssingsTheCurrentQuoteToTheLoggedInUser_WhenTheUserCreatesAQuoteAsGuest_AndThenLogsIn()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.GetAsync("/api/Quote");

            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Null(_dbContext.Quotes.First(q => q.Id == quote!.Id).CustomerId);

            await SignUp(client);

            await Login(client);

            response = await client.GetAsync("/api/Quote");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal(
                _dbContext.Users.First(u => u.Email == "test@email.com").CustomerId,
                _dbContext.Quotes.First(q => q.Id == quote!.Id).CustomerId
            );
        });
    }

    [Fact]
    public async Task GetQuote_MergesTheCurrentGuestQuoteWithAnExistingCustomerQuote_WhenTheUserCreatesAQuoteAsGuest_AndThenLogsIn_AndTheyHaveAQuoteAssociatedToTheirAccount()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var anotherNewProduct = CreateNewProduct("another_test_product");
            var yetAnotherNewProduct = CreateNewProduct("yet_another_test_product");

            // First, open up a browser, login, and create a quote
            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = anotherNewProduct.Id,
                    Quantity = 2
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.GetAsync("/api/Quote");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();
            var customerQuoteId = quote!.Id;

            // Ensure that the quote is associated with the account
            Assert.Equal(
                _dbContext.Users.First(u => u.Email == "test@email.com").CustomerId,
                _dbContext.Quotes.First(q => q.Id == quote!.Id).CustomerId
            );

            // Then, open up a second browser and create a quote as a guest
            client = CreateHttpClient();

            // Add a new item with the same product as one of the items in the logged in customer quote
            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Add a new item with a different product than the ones in the logged in customer quote
            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = yetAnotherNewProduct.Id,
                    Quantity = 4
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.GetAsync("/api/Quote");

            quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();
            var guestQuoteId = quote!.Id;

            // Ensure that this quote is not associated with an account
            Assert.Null(_dbContext.Quotes.First(q => q.Id == quote!.Id).CustomerId);

            // Ensure that there are now two different quotes
            Assert.NotEqual(customerQuoteId, guestQuoteId);
            Assert.Equal(2, _dbContext.Quotes.Count());

            // Then, login and check that the two quotes are merged
            await Login(client);

            response = await client.GetAsync("/api/Quote");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            // Assert that the response contains the correct data
            Assert.Equal(3, quote!.Items.Count);

            // Assert that one item has been merged
            var itemWithNewProduct = quote!.Items.First(i => i.ProductId == newProduct.Id);
            Assert.Equal(4, itemWithNewProduct.Quantity);

            // Assert that the item from the customer quote is included
            var itemWithAnotherNewProduct = quote!.Items.First(i => i.ProductId == anotherNewProduct.Id);
            Assert.Equal(2, itemWithAnotherNewProduct.Quantity);

            // Assert that the item from the guest quote is included
            var itemWithYetAnotherNewProduct = quote!.Items.First(i => i.ProductId == yetAnotherNewProduct.Id);
            Assert.Equal(4, itemWithYetAnotherNewProduct.Quantity);

            // Assert that the database contains the correct data
            var quoteFromDb = _dbContext.Quotes.First(q => q.Id == quote!.Id);

            Assert.Equal(
                _dbContext.Users.First(u => u.Email == "test@email.com").CustomerId,
                quoteFromDb.CustomerId
            );

            Assert.Equal(3, quoteFromDb.Items.Count);

            // Assert that one item has been merged
            var dbItemWithNewProduct = quoteFromDb!.Items.First(i => i.ProductId == newProduct.Id);
            Assert.Equal(4, itemWithNewProduct.Quantity);

            // Assert that the item from the customer quote is included
            var dbItemWithAnotherNewProduct = quoteFromDb!.Items.First(i => i.ProductId == anotherNewProduct.Id);
            Assert.Equal(2, itemWithAnotherNewProduct.Quantity);

            // Assert that the item from the guest quote is included
            var dbItemWithYetAnotherNewProduct = quoteFromDb!.Items.First(i => i.ProductId == yetAnotherNewProduct.Id);
            Assert.Equal(4, itemWithYetAnotherNewProduct.Quantity);
        });
    }

    [Fact]
    public async Task PutQuote_CreatesANewQuoteWhenNoneExists()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewCoupon();

            var client = CreateHttpClient();

            // Act
            var response = await client.PutAsJsonAsync(
                "/api/Quote",
                new {}
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.NotNull(_dbContext.Quotes.Find(quote!.Id));
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheGivenCouponAndFixedDiscountToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewFixedCoupon();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "fixed_coupon_code"
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal("fixed_coupon_code", _dbContext.Quotes.Find(quote!.Id)!.Coupon!.Code);
            Assert.Equal(4.5M, _dbContext.Quotes.Find(quote!.Id)!.Discount);
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheGivenCouponAndPercentageDiscountToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "test_coupon_code"
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal("test_coupon_code", _dbContext.Quotes.Find(quote!.Id)!.Coupon!.Code);
            Assert.Equal(3.0M, _dbContext.Quotes.Find(quote!.Id)!.Discount);
        });
    }

    [Fact]
    public async Task PutQuote_RemovesTheCouponFromTheQuote_IfNoneIsGiven()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();

            var client = CreateHttpClient();

            var response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "test_coupon_code",
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal("test_coupon_code", _dbContext.Quotes.Find(quote!.Id)!.Coupon!.Code);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = (string?)null,
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Null(_dbContext.Quotes.Find(quote!.Id)!.Coupon);
        });
    }

    [Fact]
    public async Task PutQuote_ReturnsNotFound_WhenTheGivenCouponDoesNotExist()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "non_existent_coupon"
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task PutQuote_CalculatesTheTaxOnTheQuote_WhenGivenACoupon_WhenTheQuoteHasItemsAndShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();
            CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = _dbContext.States.First().Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Reset the tax to 0.0 because other calls also update the tax.
            _dbContext.Quotes.First().Tax = 0.0M;
            _dbContext.SaveChanges();

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "test_coupon_code"
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(12.34M, _dbContext.Quotes.Find(quote!.Id)!.Tax);
        });
    }

    [Fact]
    public async Task PutQuote_DoesNotCalculateTheTaxOnTheQuote_WhenGivenACoupon_WhenTheQuoteHasNoItems()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();
            CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = _dbContext.States.First().Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "test_coupon_code"
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(0M, _dbContext.Quotes.Find(quote!.Id)!.Tax);
        });
    }

    [Fact]
    public async Task PutQuote_DoesNotCalculateTheTaxOnTheQuote_WhenGivenACoupon_WhenTheQuoteHasNoShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();
            CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    CouponCode = "test_coupon_code"
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(0M, _dbContext.Quotes.Find(quote!.Id)!.Tax);
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheGivenEmailToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    Email = "test@email.com"
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal("test@email.com", _dbContext.Quotes.Find(quote!.Id)!.Email);
        });
    }

    [Fact]
    public async Task PutQuote_ReturnsBadRequest_WhenTheGivenEmailIsInvalid()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    Email = "this is not an email"
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheGivenShippingAddressToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal("test_name", _dbContext.Quotes.First().ShippingAddress!.Name);
            Assert.Equal("test_last_name", _dbContext.Quotes.First().ShippingAddress!.LastName);
            Assert.Equal("test_street_one", _dbContext.Quotes.First().ShippingAddress!.Street);
            Assert.Equal("test_street_two", _dbContext.Quotes.First().ShippingAddress!.StreetTwo);
            Assert.Equal("test_city", _dbContext.Quotes.First().ShippingAddress!.City);
            Assert.Equal(newState.Id, _dbContext.Quotes.First().ShippingAddress!.StateId);
            Assert.Equal("12345", _dbContext.Quotes.First().ShippingAddress!.ZipCode);
            Assert.Equal("1234567890", _dbContext.Quotes.First().ShippingAddress!.PhoneNumber);
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheShippingAddressWithTheGivenIdToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();
            var newAddress = CreateNewAddress(newState);

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddressId = newAddress.Id
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal("test_name", _dbContext.Quotes.First().ShippingAddress!.Name);
            Assert.Equal("test_last_name", _dbContext.Quotes.First().ShippingAddress!.LastName);
            Assert.Equal("test_street_one", _dbContext.Quotes.First().ShippingAddress!.Street);
            Assert.Equal("test_street_two", _dbContext.Quotes.First().ShippingAddress!.StreetTwo);
            Assert.Equal("test_city", _dbContext.Quotes.First().ShippingAddress!.City);
            Assert.Equal(newState.Id, _dbContext.Quotes.First().ShippingAddress!.StateId);
            Assert.Equal("12345", _dbContext.Quotes.First().ShippingAddress!.ZipCode);
            Assert.Equal("1234567890", _dbContext.Quotes.First().ShippingAddress!.PhoneNumber);
        });
    }

    [Fact]
    public async Task PutQuote_StoresTheShippingAddressAssociatingItWithTheCustomer_IfThereIsAUserLoggedIn()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(_dbContext.Customers.First().Id, _dbContext.Quotes.First().ShippingAddress!.CustomerId);
        });
    }

    [Fact]
    public async Task PutQuote_CalculatesTheTaxOnTheQuote_WhenGivenAShippingAddress_WhenTheQuoteHasItems()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();
            CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = _dbContext.States.First().Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(12.34M, _dbContext.Quotes.Find(quote!.Id)!.Tax);
        });
    }

    [Fact]
    public async Task PutQuote_DoesNotCalculateTheTaxOnTheQuote_WhenGivenAShippingAddress_WhenTheQuoteHasNoItems()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewCoupon();
            CreateNewState();

            var client = CreateHttpClient();

            // Act
            var response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = _dbContext.States.First().Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            var quote = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.Quote>();

            Assert.Equal(0M, _dbContext.Quotes.Find(quote!.Id)!.Tax);
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheGivenBillingAddressToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    BillingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal("test_name", _dbContext.Quotes.First().BillingAddress!.Name);
            Assert.Equal("test_last_name", _dbContext.Quotes.First().BillingAddress!.LastName);
            Assert.Equal("test_street_one", _dbContext.Quotes.First().BillingAddress!.Street);
            Assert.Equal("test_street_two", _dbContext.Quotes.First().BillingAddress!.StreetTwo);
            Assert.Equal("test_city", _dbContext.Quotes.First().BillingAddress!.City);
            Assert.Equal(newState.Id, _dbContext.Quotes.First().BillingAddress!.StateId);
            Assert.Equal("12345", _dbContext.Quotes.First().BillingAddress!.ZipCode);
            Assert.Equal("1234567890", _dbContext.Quotes.First().BillingAddress!.PhoneNumber);
        });
    }

    [Fact]
    public async Task PutQuote_AssignsTheBillingAddressWithTheGivenIdToTheQuote()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();
            var newAddress = CreateNewAddress(newState);

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    BillingAddressId = newAddress.Id
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal("test_name", _dbContext.Quotes.First().BillingAddress!.Name);
            Assert.Equal("test_last_name", _dbContext.Quotes.First().BillingAddress!.LastName);
            Assert.Equal("test_street_one", _dbContext.Quotes.First().BillingAddress!.Street);
            Assert.Equal("test_street_two", _dbContext.Quotes.First().BillingAddress!.StreetTwo);
            Assert.Equal("test_city", _dbContext.Quotes.First().BillingAddress!.City);
            Assert.Equal(newState.Id, _dbContext.Quotes.First().BillingAddress!.StateId);
            Assert.Equal("12345", _dbContext.Quotes.First().BillingAddress!.ZipCode);
            Assert.Equal("1234567890", _dbContext.Quotes.First().BillingAddress!.PhoneNumber);
        });
    }

    [Fact]
    public async Task PutQuote_StoresTheBillingAddressAssociatingItWithTheCustomer_IfThereIsAUserLoggedIn()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 1
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    BillingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(_dbContext.Customers.First().Id, _dbContext.Quotes.First().BillingAddress!.CustomerId);
        });
    }

    [Fact]
    public async Task PostQuoteItem_CreatesANewQuoteAndANewItem()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());

            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(_dbContext.Quotes.First().Id, _dbContext.QuoteItems.First().QuoteId);
            Assert.Equal(newProduct.Id, _dbContext.QuoteItems.First().ProductId);
            Assert.Equal(3, _dbContext.QuoteItems.First().Quantity);
        });
    }

    [Fact]
    public async Task PostQuoteItem_CreatesANewQuoteItemButNotANewQuote_WhenAQuoteAlreadyExists()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var anotherNewProduct = CreateNewProduct("another_test_product");

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = anotherNewProduct.Id,
                    Quantity = 5
                }
            );

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());

            Assert.Equal(2, _dbContext.QuoteItems.ToList().Count);
            Assert.Equal(_dbContext.Quotes.First().Id, _dbContext.QuoteItems.ToList().First().QuoteId);
            Assert.Equal(_dbContext.Quotes.First().Id, _dbContext.QuoteItems.ToList().Last().QuoteId);

            Assert.Contains(newProduct.Id, _dbContext.QuoteItems.ToList().Select(i => i.ProductId));
            Assert.Contains(3, _dbContext.QuoteItems.ToList().Select(i => i.Quantity));

            Assert.Contains(anotherNewProduct.Id, _dbContext.QuoteItems.ToList().Select(i => i.ProductId));
            Assert.Contains(5, _dbContext.QuoteItems.ToList().Select(i => i.Quantity));
        });
    }

    [Fact]
    public async Task PostQuoteItem_RespondsWithTheNewlyCreatedQuoteItemData()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            Assert.Equal(newProduct.Id, quoteItem!.ProductId);
            Assert.Equal(3, quoteItem!.Quantity);
            Assert.Equal(10.00M, quoteItem!.UnitPrice);
            Assert.Equal(30.00M, quoteItem!.Total);
        });
    }

    [Fact]
    public async Task PostQuoteItem_RespondsWithACookieThatContainsTheQuoteId()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(response.Headers);
            Assert.Equal("Set-Cookie", response.Headers.First().Key);
            Assert.Contains("EndPointCommerce_QuoteId=", response.Headers.First().Value.First());
        });
    }

    [Fact]
    public async Task PostQuoteItem_IncreasesTheQuantityOfAnExistingQuoteItem_IfItMatchesTheGivenProduct()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 2
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Single(_dbContext.Quotes.ToList());

            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(_dbContext.Quotes.First().Id, _dbContext.QuoteItems.First().QuoteId);
            Assert.Equal(newProduct.Id, _dbContext.QuoteItems.First().ProductId);
            Assert.Equal(5, _dbContext.QuoteItems.First().Quantity);
        });
    }

    [Fact]
    public async Task PostQuoteItem_CalculatesTheTaxOfTheQuote_WhenTheQuoteHasShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Equal(12.34M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task PostQuoteItem_CalculatesTheTaxOfTheQuote_WhenTheGivenItemMatchesAnExistingOne_WhenTheQuoteHasShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _dbContext.Quotes.First().Tax = 0.0M;
            _dbContext.SaveChanges();

            // Act
            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 5
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(12.34M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task PostQuoteItem_DoesNotCalculateTheTaxOfTheQuote_WhenTheQuoteHasNoShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Equal(0M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task PostQuoteItem_RespondsWithNotFound_IfTheGivenProductIdDoesNotExist()
    {
        await WithTransaction(async () => {
            // Arrange
            var client = CreateHttpClient();

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = -1,
                    Quantity = 1
                }
            );

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task PutQuoteItem_UpdatesTheGivenQuoteItem()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            // Act
            response = await client.PutAsJsonAsync(
                $"/api/Quote/Items/{quoteItem!.Id}",
                new
                {
                    Quantity = 5
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());

            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(_dbContext.Quotes.First().Id, _dbContext.QuoteItems.First().QuoteId);
            Assert.Equal(newProduct.Id, _dbContext.QuoteItems.First().ProductId);
            Assert.Equal(5, _dbContext.QuoteItems.First().Quantity);
        });
    }

    [Fact]
    public async Task PutQuoteItem_DeletesTheGivenQuoteItem_IfTheGivenQuantityIsZero()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            // Act
            response = await client.PutAsJsonAsync(
                $"/api/Quote/Items/{quoteItem!.Id}",
                new
                {
                    Quantity = 0
                }
            );

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());

            Assert.Empty(_dbContext.QuoteItems.ToList());
        });
    }

    [Fact]
    public async Task PutQuoteItem_CalculatesTheTaxOfTheQuote_WhenTheQuoteHasShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = newState.Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _dbContext.Quotes.First().Tax = 0.0M;
            _dbContext.SaveChanges();

            // Act
            response = await client.PutAsJsonAsync(
                $"/api/Quote/Items/{quoteItem!.Id}",
                new
                {
                    Quantity = 5
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Equal(12.34M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task PutQuoteItem_DoesNotCalculateTheTaxOfTheQuote_WhenTheQuoteHasNoShippingAddress()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var newState = CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            // Act
            response = await client.PutAsJsonAsync(
                $"/api/Quote/Items/{quoteItem!.Id}",
                new
                {
                    Quantity = 5
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert
            Assert.Equal(0M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task DeleteQuoteItem_DeletesTheGivenQuoteItem()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            // Act
            response = await client.DeleteAsync(
                $"/api/Quote/Items/{quoteItem!.Id}"
            );

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Empty(_dbContext.QuoteItems.ToList());
        });
    }

    [Fact]
    public async Task DeleteQuoteItem_CalculatesTheTaxOfTheQuote_WhenTheQuoteHasShippingAddress_WhenTheQuoteHasItems()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            var anotherNewProduct = CreateNewProduct("another_test_product");
            CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = anotherNewProduct.Id,
                    Quantity = 5
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = _dbContext.States.First().Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _dbContext.Quotes.First().Tax = 0.0M;
            _dbContext.SaveChanges();

            // Act
            response = await client.DeleteAsync(
                $"/api/Quote/Items/{quoteItem!.Id}"
            );

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Single(_dbContext.QuoteItems.ToList());
            Assert.Equal(12.34M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task DeleteQuoteItem_SetsTheTaxOfTheQuoteToZero_WhenTheQuoteHasNoItemsLeft()
    {
        await WithTransaction(async () => {
            // Arrange
            var newProduct = CreateNewProduct();
            CreateNewState();

            var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync(
                "/api/Quote/Items",
                new
                {
                    ProductId = newProduct.Id,
                    Quantity = 3
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var quoteItem = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.QuoteItem>();

            response = await client.PutAsJsonAsync(
                "/api/Quote",
                new
                {
                    ShippingAddress = new
                    {
                        Name = "test_name",
                        LastName = "test_last_name",
                        Street = "test_street_one",
                        StreetTwo = "test_street_two",
                        City = "test_city",
                        StateId = _dbContext.States.First().Id,
                        ZipCode = "12345",
                        PhoneNumber = "1234567890"
                    }
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            response = await client.DeleteAsync(
                $"/api/Quote/Items/{quoteItem!.Id}"
            );

            // Assert
            Assert.Single(_dbContext.Quotes.ToList());
            Assert.Empty(_dbContext.QuoteItems.ToList());
            Assert.Equal(0M, _dbContext.Quotes.First().Tax);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsOk_WhenTheQuoteIsValid()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(client);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsNotFound_WhenTheQuoteIsNotOpen()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            var response = await CreateAndValidateQuote(client);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _dbContext.Quotes.First().Close();
            _dbContext.SaveChanges();

            // Act
            response = await client.PostAsJsonAsync("api/Quote/Validate", new { });

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsBadRequest_WhenTheQuoteHasNoItems()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            var response = await CreateAndValidateQuote(client);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _dbContext.Quotes.First().Items.Clear();
            _dbContext.SaveChanges();

            // Act
            response = await client.PostAsJsonAsync("api/Quote/Validate", new { });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();
            Assert.NotNull(result);
            Assert.Contains("The quote is not valid.", result.Values);
            Assert.Contains("The quote must have at least one item.", result.Values);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsBadRequest_WhenTheQuoteHasNoEmail_AndTheQuoteBelongsToAGuest()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(client, orderEmail: null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();
            Assert.NotNull(result);
            Assert.Contains("The quote is not valid.", result.Values);
            Assert.Contains("An email is required for guest orders.", result.Values);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsOk_WhenTheQuoteHasNoEmail_AndTheQuoteBelongsToACustomer()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            await SignUp(client);

            await Login(client);

            // Act
            var response = await CreateAndValidateQuote(client, orderEmail: null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsBadRequest_WhenNoShippingAddressIsGiven()
    {
        await WithTransaction(async () => {
            // Arrange
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(
                client,
                useShippingAddressId: false,
                useShippingAddress: false
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();
            Assert.NotNull(result);
            Assert.Contains("The quote is not valid.", result.Values);
            Assert.Contains("The ShippingAddress field is required.", result.Values);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_AcceptsAnAddressIdForTheShippingAddress()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(
                client,
                useShippingAddressId: true,
                useShippingAddress: false
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_AcceptsAnAddresObjectForTheShippingAddress()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(
                client,
                useShippingAddressId: false,
                useShippingAddress: true
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_ReturnsBadRequest_WhenNoBillingAddressIsGiven()
    {
        await WithTransaction(async () => {
            // Arrange
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(
                client,
                useBillingAddressId: false,
                useBillingAddress: false
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>();
            Assert.NotNull(result);
            Assert.Contains("The quote is not valid.", result.Values);
            Assert.Contains("The BillingAddress field is required.", result.Values);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_AcceptsAnAddresIdForTheBillingAddress()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(
                client,
                useBillingAddressId: true,
                useBillingAddress: false
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }

    [Fact]
    public async Task PostQuoteValidate_AcceptsAnAddresObjectForTheBillingAddress()
    {
        await WithTransaction(async () =>
        {
            var client = CreateHttpClient();

            // Act
            var response = await CreateAndValidateQuote(
                client,
                useBillingAddressId: false,
                useBillingAddress: true
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });
    }
}
