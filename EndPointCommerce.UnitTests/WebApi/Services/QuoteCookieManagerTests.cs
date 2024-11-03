using EndPointCommerce.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointCommerce.UnitTests.WebApi.Services;

public class QuoteCookieManagerTests
{
    private readonly Mock<IDataProtectorProxy> _mockDataProtector;
    private readonly QuoteCookieManager _subject;

    public QuoteCookieManagerTests()
    {
        _mockDataProtector = new Mock<IDataProtectorProxy>();
        _subject = new QuoteCookieManager(_mockDataProtector.Object);
    }

    [Fact]
    public void GetQuoteIdFromCookie_ReturnsTheQuoteIdFromTheCookie()
    {
        // Arrange
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies
            .Setup(m => m["EndPointCommerce_QuoteId"])
            .Returns("test_protected_quote_id");

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(m => m.Cookies).Returns(mockCookies.Object);

        _mockDataProtector
            .Setup(x => x.Unprotect("test_protected_quote_id"))
            .Returns("123");

        // Act
        var result = _subject.GetQuoteIdFromCookie(mockRequest.Object);

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void GetQuoteIdFromCookie_ReturnsNull_WhenTheCookieDoesNotExist()
    {
        // Arrange
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies
            .Setup(x => x["EndPointCommerce_QuoteId"])
            .Returns((string?)null);

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Cookies).Returns(mockCookies.Object);

        // Act
        var result = _subject.GetQuoteIdFromCookie(mockRequest.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SetQuoteIdCookie_SetsTheQuoteIdCookie()
    {
        // Arrange
        _mockDataProtector
            .Setup(x => x.Protect("123"))
            .Returns("test_protected_quote_id");

        var mockCookies = new Mock<IResponseCookies>();
        mockCookies.Setup(m => m.Append("EndPointCommerce_QuoteId", "test_protected_quote_id"));

        var mockResponse = new Mock<HttpResponse>();
        mockResponse.Setup(m => m.Cookies).Returns(mockCookies.Object);

        // Act
        _subject.SetQuoteIdCookie(mockResponse.Object, 123);

        // Assert
        mockResponse.Verify(x => x.Cookies.Append(
            "EndPointCommerce_QuoteId",
            "test_protected_quote_id",
            // Check that the date is pretty much 7 days from now
            It.Is<CookieOptions>(o =>
                (DateTimeOffset.Now.AddDays(7) - o.Expires!).Value.TotalSeconds < 1
            )
        ));
    }

    [Fact]
    public void DeleteQuoteIdCookie_DeletesTheQuoteIdCookie()
    {
        // Arrange
        var mockCookies = new Mock<IResponseCookies>();

        var mockResponse = new Mock<HttpResponse>();
        mockResponse.Setup(m => m.Cookies).Returns(mockCookies.Object);

        // Act
        _subject.DeleteQuoteIdCookie(mockResponse.Object);

        // Assert
        mockResponse.Verify(x => x.Cookies.Delete("EndPointCommerce_QuoteId"));
    }
}
