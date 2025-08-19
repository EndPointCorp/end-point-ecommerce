// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.WebApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Moq;

namespace EndPointEcommerce.Tests.WebApi.Services;

public class DataProtectorProxyTests
{
    [Fact]
    public void Constructor_CreatesADataProtectorUsingTheProperParameter()
    {
        // Arrange
        var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
        mockDataProtectionProvider
            .Setup(x => x.CreateProtector("EndPointEcommerceCookieProtector"))
            .Returns(Mock.Of<IDataProtector>());

        // Act
        _ = new DataProtectorProxy(mockDataProtectionProvider.Object);

        // Assert
        mockDataProtectionProvider.Verify(x => x.CreateProtector("EndPointEcommerceCookieProtector"), Times.Once);
    }
}