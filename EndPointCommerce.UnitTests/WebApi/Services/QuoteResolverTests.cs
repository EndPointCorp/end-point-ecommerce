using System.Security.Principal;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointCommerce.UnitTests.WebApi.Services;

public class QuoteResolverTests
{
    private readonly Mock<IQuoteRepository> _mockQuoteRepository;
    private readonly Mock<IQuoteCookieManager> _mockQuoteCookieManager;
    private readonly Mock<ISessionHelper> _mockSessionHelper;
    private readonly Mock<IPrincipal> _mockPrincipal;
    private readonly Mock<HttpRequest> _mockRequest;
    private readonly QuoteResolver _subject;

    public QuoteResolverTests()
    {
        _mockQuoteRepository = new Mock<IQuoteRepository>();
        _mockQuoteCookieManager = new Mock<IQuoteCookieManager>();
        _mockSessionHelper = new Mock<ISessionHelper>();

        _mockPrincipal = new Mock<IPrincipal>();
        _mockRequest = new Mock<HttpRequest>();

        _subject = new QuoteResolver(
            _mockQuoteRepository.Object,
            _mockQuoteCookieManager.Object,
            _mockSessionHelper.Object
        );
    }

    [Fact]
    public async Task ResolveQuote_ReturnsTheCookieQuote_WhenTheUserIsNotAuthenticated_AndThereIsAQuoteIdCookie()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(false);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns(123);

        var quote = new Quote { Id = 123 };

        _mockQuoteRepository
            .Setup(m => m.FindOpenByIdAsync(123))
            .ReturnsAsync(quote);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Equal(quote, result);
    }

    [Fact]
    public async Task ResolveQuote_ReturnsNull_WhenTheUserIsNotAuthenticated_AndThereIsAQuoteIdCookie_AndThereIsNoQuoteWithThatId()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(false);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns(123);

        _mockQuoteRepository
            .Setup(m => m.FindOpenByIdAsync(123))
            .ReturnsAsync((Quote?)null);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveQuote_ReturnsNull_WhenTheUserIsNotAuthenticated_AndThereIsNoQuoteIdCookie()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(false);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns((int?)null);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveQuote_ReturnsTheCustomerQuote_WhenTheUserIsAuthenticated_AndThereIsNoQuoteIdCookie()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(true);

        _mockSessionHelper
            .Setup(m => m.GetCustomerId(_mockPrincipal.Object))
            .ReturnsAsync(100);

        var quote = new Quote { Id = 123 };

        _mockQuoteRepository
            .Setup(m => m.FindOpenByCustomerIdAsync(100))
            .ReturnsAsync(quote);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns((int?)null);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Equal(quote, result);
    }

    [Fact]
    public async Task ResolveQuote_ReturnsNull_WhenTheUserIsAuthenticated_AndThereIsNoCustomerQuote_AndThereIsNoQuoteIdCookie()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(true);

        _mockSessionHelper
            .Setup(m => m.GetCustomerId(_mockPrincipal.Object))
            .ReturnsAsync(100);

        _mockQuoteRepository
            .Setup(m => m.FindOpenByCustomerIdAsync(100))
            .ReturnsAsync((Quote?)null);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns((int?)null);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveQuote_ThrowsEntityNotFoundException_WhenTheUserIsAuthenticated_AndTheAuthenticatedUserHasNoCustomerId()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(true);

        _mockSessionHelper
            .Setup(m => m.GetCustomerId(_mockPrincipal.Object))
            .ReturnsAsync((int?)null);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object));
    }

    [Fact]
    public async Task ResolveQuote_ReturnsTheCookieQuote_AndAssignsItToTheAuthenticatedUser_WhenTheUserIsAuthenticated_AndThereIsNoCustomerQuote_AndThereIsACookieQuote()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(true);

        _mockSessionHelper
            .Setup(m => m.GetCustomerId(_mockPrincipal.Object))
            .ReturnsAsync(100);

        _mockQuoteRepository
            .Setup(m => m.FindOpenByCustomerIdAsync(100))
            .ReturnsAsync((Quote?)null);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns(123);

        var quote = new Quote { Id = 123 };

        _mockQuoteRepository
            .Setup(m => m.FindOpenByIdAsync(123))
            .ReturnsAsync(quote);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Equal(quote, result);
        _mockQuoteRepository.Verify(
            q => q.UpdateAsync(
                It.Is<Quote>(q => q.CustomerId == 100 && q.Id == 123)
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ResolveQuote_ReturnsTheCustomerQuote_AndMergesTheCookieQuoteIntoIt_AndClosesTheCookieQuote_WhenTheUserIsAuthenticated_AndThereIsACustomerQuote_AndThereIsACookieQuote()
    {
        // Arrange
        _mockSessionHelper
            .Setup(m => m.IsAuthenticated(_mockPrincipal.Object))
            .Returns(true);

        _mockSessionHelper
            .Setup(m => m.GetCustomerId(_mockPrincipal.Object))
            .ReturnsAsync(100);

        var customerQuote = new Quote
        {
            Id = 123,
            Items =
            [
                new QuoteItem { ProductId = 1, Quantity = 1 }
            ]
        };

        _mockQuoteRepository
            .Setup(m => m.FindOpenByCustomerIdAsync(100))
            .ReturnsAsync(customerQuote);

        _mockQuoteCookieManager
            .Setup(m => m.GetQuoteIdFromCookie(_mockRequest.Object))
            .Returns(456);

        var cookieQuote = new Quote
        {
            Id = 456,
            Items =
            [
                new QuoteItem { ProductId = 2, Quantity = 2 }
            ]
        };

        _mockQuoteRepository
            .Setup(m => m.FindOpenByIdAsync(456))
            .ReturnsAsync(cookieQuote);

        // Act
        var result = await _subject.ResolveQuote(_mockPrincipal.Object, _mockRequest.Object);

        // Assert
        Assert.Equal(customerQuote, result);

        _mockQuoteRepository.Verify(q => q.UpdateAsync(customerQuote), Times.Once);
        _mockQuoteRepository.Verify(q => q.UpdateAsync(cookieQuote), Times.Once);

        Assert.Equal(2, customerQuote.Items.Count);
        Assert.Contains(customerQuote.Items, i => i.ProductId == 1 && i.Quantity == 1);
        Assert.Contains(customerQuote.Items, i => i.ProductId == 2 && i.Quantity == 2);
        Assert.False(cookieQuote.IsOpen);
    }
}
