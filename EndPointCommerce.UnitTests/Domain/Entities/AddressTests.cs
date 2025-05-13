using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.UnitTests.Domain.Entities;

public class AddressTest
{
    [Fact]
    public void FullAddress_ShouldReturnCorrectFormat()
    {
        // Arrange
        var address = new Address
        {
            Name = "John",
            LastName = "Doe",
            Street = "123 Main St",
            City = "Anytown",
            ZipCode = "12345",
            Country = new Country { Name = "United States", Code = "US" },
            CountryId = 1,
            State = new State { Name = "New York", Abbreviation = "NY" },
            StateId = 1
        };

        // Act
        var fullAddress = address.FullAddress;

        // Assert
        Assert.Equal("123 Main St, Anytown, New York, 12345, United States", fullAddress);
    }

    [Fact]
    public void FullName_ShouldReturnCorrectFullName()
    {
        // Arrange
        var address = new Address
        {
            Name = "John",
            LastName = "Doe",
            Street = "123 Main St",
            City = "Anytown",
            ZipCode = "12345",
            Country = new Country { Name = "United States", Code = "US" },
            CountryId = 1,
            State = new State { Name = "New York", Abbreviation = "NY" },
            StateId = 1
        };

        // Act
        var fullName = address.FullName;

        // Assert
        Assert.Equal("John Doe", fullName);
    }

    [Fact]
    public void Clone_ShouldCreateANewObjectWithTheSameProperties()
    {
        // Arrange
        var state = new State { Name = "New York", Abbreviation = "NY" };
        var address = new Address
        {
            Name = "John",
            LastName = "Doe",
            PhoneNumber = "123-456-7890",
            Street = "123 Main St",
            StreetTwo = "Apt 4",
            City = "Anytown",
            ZipCode = "12345",
            Country = new Country { Name = "United States", Code = "US" },
            CountryId = 1,
            State = new State { Name = "New York", Abbreviation = "NY" },
            StateId = 1
        };

        // Act
        var clonedAddress = address.Clone();

        // Assert
        Assert.Equal(address.Name, clonedAddress.Name);
        Assert.Equal(address.LastName, clonedAddress.LastName);
        Assert.Equal(address.PhoneNumber, clonedAddress.PhoneNumber);
        Assert.Equal(address.Street, clonedAddress.Street);
        Assert.Equal(address.StreetTwo, clonedAddress.StreetTwo);
        Assert.Equal(address.City, clonedAddress.City);
        Assert.Equal(address.ZipCode, clonedAddress.ZipCode);
        Assert.Equal(address.Country, clonedAddress.Country);
        Assert.Equal(address.CountryId, clonedAddress.CountryId);
        Assert.Equal(address.State, clonedAddress.State);
        Assert.Equal(address.StateId, clonedAddress.StateId);
    }

}