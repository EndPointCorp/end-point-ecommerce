using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Tests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointCommerce.Tests.WebApi.Controllers;

public class AddressesControllerTests : IntegrationTests
{
    public AddressesControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private Address CreateNewAddress(State state, Customer customer, bool deleted = false)
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
            PhoneNumber = "1234567890",
            CustomerId = customer.Id,

            Deleted = deleted
        };

        dbContext.Addresses.Add(newAddress);
        dbContext.SaveChanges();

        return newAddress;
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

    [Fact]
    public async Task GetAddresses_ReturnsAddresses_AssociatedWithLoggedInUser_ExcludingDeletedOnes()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        var customer = dbContext.Customers.First();
        var address1 = CreateNewAddress(dbContext.States.First(), customer);
        var address2 = CreateNewAddress(dbContext.States.Skip(5).First(), customer);
        var address3 = CreateNewAddress(dbContext.States.Skip(10).First(), customer, true);

        // Act
        var response = await client.GetAsync("/api/Addresses");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var addresses = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Address>>();
        Assert.Equal(2, addresses!.Count());
        Assert.Contains(addresses!, a => a.Id == address1.Id);
        Assert.Contains(addresses!, a => a.Id == address2.Id);
        Assert.DoesNotContain(addresses!, a => a.Id == address3.Id);
    }

    [Fact]
    public async Task GetAddresses_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Addresses");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAddresses_ReturnsEmptyList_WhenLoggedInUserHasNoAddresses()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);

        await Login(client);

        // Act
        var response = await client.GetAsync("/api/Addresses");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var addresses = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Address>>();
        Assert.Empty(addresses!);
    }

    [Fact]
    public async Task PostAddress_CreatesNewAddress_AssociatedWithLoggedInUser()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Addresses",
            new
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "test_street_one",
                StreetTwo = "test_street_two",
                City = "test_city",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First().Id,
                ZipCode = "12345",
                PhoneNumber = "1234567890"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var address = dbContext.Addresses.First();

        Assert.NotNull(address);
        Assert.Equal("test_name", address.Name);
        Assert.Equal("test_last_name", address.LastName);
        Assert.Equal("test_street_one", address.Street);
        Assert.Equal("test_street_two", address.StreetTwo);
        Assert.Equal("test_city", address.City);
        Assert.Equal(dbContext.States.First().Id, address.StateId);
        Assert.Equal("12345", address.ZipCode);
        Assert.Equal(dbContext.Customers.First().Id, address.CustomerId);
    }

    [Fact]
    public async Task PostAddress_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Addresses",
            new
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "test_street_one",
                StreetTwo = "test_street_two",
                City = "test_city",
                StateId = dbContext.States.First().Id,
                ZipCode = "12345",
                PhoneNumber = "1234567890"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutAddress_UpdatesAddress()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        var address = CreateNewAddress(
            dbContext.States.First(),
            dbContext.Customers.First()
        );

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/Addresses/{address.Id}", new
            {
                Name = "updated_name",
                LastName = "updated_last_name",
                Street = "updated_street",
                City = "updated_city",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.Skip(10).First().Id,
                ZipCode = "54321",
                PhoneNumber = "0987654321"
            });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedAddress = dbContext.Addresses.Find(address.Id);
        Assert.Equal("updated_name", updatedAddress!.Name);
        Assert.Equal("updated_last_name", updatedAddress.LastName);
        Assert.Equal("updated_street", updatedAddress.Street);
        Assert.Equal("updated_city", updatedAddress.City);
        Assert.Equal(dbContext.States.Skip(10).First().Id, updatedAddress.StateId);
        Assert.Equal("54321", updatedAddress.ZipCode);
        Assert.Equal("0987654321", updatedAddress.PhoneNumber);
    }

    [Fact]
    public async Task PutAddress_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/Addresses/1",
            new
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "test_street_one",
                StreetTwo = "test_street_two",
                City = "test_city",
                StateId = dbContext.States.First().Id,
                ZipCode = "12345",
                PhoneNumber = "1234567890"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutAddress_ReturnsNotFound_WhenAddressDoesNotExist()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/Addresses/9999",
            new
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "test_street_one",
                StreetTwo = "test_street_two",
                City = "test_city",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First().Id,
                ZipCode = "12345",
                PhoneNumber = "1234567890"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutAddress_ReturnsNotFound_WhenAddressDoesNotBelongToLoggedInUser()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        var anotherCustomer = new Customer { Name = "test_name", Email = "test@email.com" };
        dbContext.Customers.Add(anotherCustomer);
        dbContext.SaveChanges();

        var address = CreateNewAddress(
            dbContext.States.First(),
            anotherCustomer
        );

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/Addresses/{address.Id}",
            new
            {
                Name = "test_name",
                LastName = "test_last_name",
                Street = "test_street_one",
                StreetTwo = "test_street_two",
                City = "test_city",
                CountryId = dbContext.Countries.First(c => c.Code == "US").Id,
                StateId = dbContext.States.First().Id,
                ZipCode = "12345",
                PhoneNumber = "1234567890"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_DeletesAddress()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        var address = CreateNewAddress(
            dbContext.States.First(),
            dbContext.Customers.First()
        );

        // Act
        var response = await client.DeleteAsync($"/api/Addresses/{address.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deletedAddress = dbContext.Addresses.Find(address.Id);
        Assert.True(deletedAddress!.Deleted);
    }

    [Fact]
    public async Task DeleteAddress_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.DeleteAsync($"/api/Addresses/{123}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ReturnsNotFound_WhenAddressDoesNotExist()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.DeleteAsync($"/api/Addresses/{123}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ReturnsNotFound_WhenAddressDoesNotBelongToLoggedInUser()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        var anotherCustomer = new Customer { Name = "test_name", Email = "test@email.com" };
        dbContext.Customers.Add(anotherCustomer);
        dbContext.SaveChanges();

        var address = CreateNewAddress(
            dbContext.States.First(),
            anotherCustomer
        );

        // Act
        var response = await client.DeleteAsync($"/api/Addresses/{address.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
