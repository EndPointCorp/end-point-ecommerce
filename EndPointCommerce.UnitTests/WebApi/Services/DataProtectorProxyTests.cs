using EndPointCommerce.Domain.Entities;
using EndPointCommerce.WebApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Moq;

namespace EndPointCommerce.UnitTests.WebApi.Services;

public class DataProtectorProxyTests
{
    [Fact]
    public void Constructor_CreatesADataProtectorUsingTheProperParameter()
    {
        // Arrange
        var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
        mockDataProtectionProvider
            .Setup(x => x.CreateProtector("EndPointCommerceCookieProtector"))
            .Returns(Mock.Of<IDataProtector>());

        // Act
        _ = new DataProtectorProxy(mockDataProtectionProvider.Object);

        // Assert
        mockDataProtectionProvider.Verify(x => x.CreateProtector("EndPointCommerceCookieProtector"), Times.Once);
    }
}