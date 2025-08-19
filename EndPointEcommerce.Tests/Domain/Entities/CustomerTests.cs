// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Tests.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void FullName_ShouldReturnFirstNameAndLastName()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Assert
        Assert.Equal("John Doe", customer.FullName);
    }

    [Fact]
    public void FullName_ShouldReturnFirstName_WhenLastNameIsNull()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John",
            LastName = null,
            Email = "john.doe@example.com"
        };

        // Assert
        Assert.Equal("John", customer.FullName);
    }

}
