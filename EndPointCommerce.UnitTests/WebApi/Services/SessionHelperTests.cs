using System.Security.Principal;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.WebApi.Services;
using Moq;

namespace EndPointCommerce.UnitTests.WebApi.Services;

public class SessionHelperTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly ISessionHelper _subject;

    public SessionHelperTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _subject = new SessionHelper(_mockIdentityService.Object);
    }

    [Fact]
    public async Task GetCustomerId_ReturnsCustomerId_WhenUserIsAuthenticated()
    {
        // Arrange
        var mockIdentity = new Mock<IIdentity>();
        mockIdentity.Setup(m => m.IsAuthenticated).Returns(true);
        mockIdentity.Setup(m => m.Name).Returns("test_user_name");

        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(m => m.Identity).Returns(mockIdentity.Object);

        _mockIdentityService
            .Setup(service => service.FindByUserNameAsync("test_user_name"))
            .ReturnsAsync(new User { CustomerId = 123 });

        // Act
        var result = await _subject.GetCustomerId(mockPrincipal.Object);

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public async Task GetCustomerId_ThrowsEntityNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var mockIdentity = new Mock<IIdentity>();
        mockIdentity.Setup(m => m.IsAuthenticated).Returns(true);
        mockIdentity.Setup(m => m.Name).Returns("test_user_name");

        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(m => m.Identity).Returns(mockIdentity.Object);

        _mockIdentityService
            .Setup(service => service.FindByUserNameAsync("test_user_name"))
            .ReturnsAsync((User?)null);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _subject.GetCustomerId(mockPrincipal.Object));
    }

    [Fact]
    public async Task GetCustomerId_ReturnsNull_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var mockIdentity = new Mock<IIdentity>();
        mockIdentity.Setup(m => m.IsAuthenticated).Returns(false);

        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(m => m.Identity).Returns(mockIdentity.Object);

        // Act
        var result = await _subject.GetCustomerId(mockPrincipal.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCustomerId_ReturnsNull_WhenUserIsNull()
    {
        // Arrange
        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(m => m.Identity).Returns((IIdentity?)null);

        // Act
        var result = await _subject.GetCustomerId(mockPrincipal.Object);

        // Assert
        Assert.Null(result);
    }
}
