using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.UnitTests.Domain.Entities;

public class UserTest
{
    [Fact]
    public void Greeting_ShouldReturnCustomerFullName_WhenCustomerIsNotNull()
    {
        // Arrange
        var user = new User
        {
            Customer = new Customer
            {
                Name = "test_name",
                LastName = "test_last_name",
                Email = "test@email.com"
            }
        };

        // Assert
        Assert.Equal("test_name test_last_name", user.Greeting);
    }

    [Fact]
    public void Greeting_ShouldReturnEmail_WhenCustomerIsNull()
    {
        // Arrange
        var user = new User { Email = "test@email.com" };

        // Act
        var greeting = user.Greeting;

        // Assert
        Assert.Equal("test@email.com", greeting);
    }

    [Fact]
    public void IsCustomer_ShouldReturnTrue_WhenCustomerIdHasValue()
    {
        // Arrange
        var user = new User
        {
            CustomerId = 123
        };

        // Assert
        Assert.True(user.IsCustomer);
    }

    [Fact]
    public void IsCustomer_ShouldReturnFalse_WhenCustomerIdHasNoValue()
    {
        // Arrange
        var user = new User
        {
            CustomerId = null
        };

        // Assert
        Assert.False(user.IsCustomer);
    }
}
