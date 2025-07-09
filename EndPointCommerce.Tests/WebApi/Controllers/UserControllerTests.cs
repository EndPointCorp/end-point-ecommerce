using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Tests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Tests.WebApi.Controllers;

public class UserControllerTests : IntegrationTests
{
    public UserControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

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
    public async Task GetUser_ReturnsLoggedInUserData_WhenUserIsLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.GetAsync("/api/User");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<EndPointCommerce.WebApi.ResourceModels.User>();

        Assert.NotNull(user);
        Assert.Equal("test@email.com", user.Email);
        Assert.Equal(dbContext.Customers.First().Id, user.Customer!.Id);
        Assert.Equal(dbContext.Customers.First().Name, user.Customer!.Name);
        Assert.Equal(dbContext.Customers.First().LastName, user.Customer!.LastName);
    }

    [Fact]
    public async Task GetUser_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/User");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserIsNotACustomer()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Simulate a user that is not a customer
        var user = dbContext.Users.First(u => u.Email == "test@email.com");
        user.CustomerId = null;
        dbContext.SaveChanges();

        // Act
        var response = await client.GetAsync("/api/User");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostUser_CreatesNewUser()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
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

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = dbContext.Users
            .Include(u => u.Customer)
            .FirstOrDefault(u => u.Email == "test@email.com");

        Assert.NotNull(user);

        Assert.Equal(dbContext.Customers.First().Id, user.Customer!.Id);
        Assert.Equal("test_name", user.Customer!.Name);
        Assert.Equal("test_last_name", user.Customer!.LastName);
    }

    [Fact]
    public async Task PostUser_ReturnsBadRequest_WhenGivenInvalidEmail()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/User",
            new
            {
                Email = "invalid-email",
                Password = "TEST_password_123",
                Name = "test_name",
                LastName = "test_last_name"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("Email 'invalid-email' is invalid.", errorMessage);
    }

    [Fact]
    public async Task PostUser_ReturnsBadRequest_WhenGivenWeakPassword()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/User",
            new
            {
                Email = "test@email.com",
                Password = "weak",
                Name = "test_name",
                LastName = "test_last_name"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("Passwords must be at least 6 characters.", errorMessage);
        Assert.Contains("Passwords must have at least one non alphanumeric character.", errorMessage);
        Assert.Contains("Passwords must have at least one digit ('0'-'9').", errorMessage);
        Assert.Contains("Passwords must have at least one uppercase ('A'-'Z').", errorMessage);
    }

    [Fact]
    public async Task PostUser_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var client = CreateHttpClient();

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


        // Act
        response = await client.PostAsJsonAsync(
            "/api/User",
            new
            {
                Email = "test@email.com",
                Password = "Another_TEST_password_123",
                Name = "test_name",
                LastName = "test_last_name"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("Username 'test@email.com' is already taken.", errorMessage);
        Assert.Contains("Email 'test@email.com' is already taken.", errorMessage);
    }

    [Fact]
    public async Task PostLogout_LogsOutTheCurrentUser_AndClearsTheCookie()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        var response = await client.GetAsync("/api/User");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Act
        response = await client.PostAsync("/api/User/Logout", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert
        response = await client.GetAsync("/api/User");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostLogout_ReturnsOk_EvenWhenNoUserIsLoggedIn()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.PostAsync("/api/User/Logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PutUser_UpdatesUserInformation()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/User",
            new
            {
                Email = "updated_test@email.com",
                PhoneNumber = "1234567890",
                Name = "updated_name",
                LastName = "updated_last_name",
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = dbContext.Users
            .Include(u => u.Customer)
            .First(u => u.Email == "updated_test@email.com");

        Assert.Equal("1234567890", user.PhoneNumber);
        Assert.Equal("updated_test@email.com", user.Customer!.Email);
        Assert.Equal("updated_name", user.Customer!.Name);
        Assert.Equal("updated_last_name", user.Customer!.LastName);
    }

    [Fact]
    public async Task PutUser_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/User",
            new
            {
                Email = "test@email.com",
                Name = "updated_name",
                LastName = "updated_last_name",
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutUser_UpdatesPassword()
    {
        // Arrange
        var client = CreateHttpClient();

        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/User",
            new
            {
                Email = "test@email.com",
                Name = "updated_name",
                LastName = "updated_last_name",
                CurrentPassword = "TEST_password_123",
                NewPassword = "NEW_password_456"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify old password does not work
        response = await client.PostAsJsonAsync(
            "/api/User/login?useCookies=true&useSessionCookies=false",
            new
            {
                Email = "test@email.com",
                Password = "TEST_password_123",
            }
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        // Verify new password works
        var loginResponse = await client.PostAsJsonAsync(
            "/api/User/login?useCookies=true&useSessionCookies=false",
            new
            {
                Email = "test@email.com",
                Password = "NEW_password_456",
            }
        );

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task PutUser_ReturnsBadRequest_WhenAttemptingToChangePassword_WhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var client = CreateHttpClient();
        await SignUp(client);
        await Login(client);

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/User",
            new
            {
                Email = "test@email.com",
                Name = "updated_name",
                LastName = "updated_last_name",
                CurrentPassword = "WRONG_password",
                NewPassword = "NEW_password_456"
            }
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("Current password is incorrect", errorMessage);
    }
}
