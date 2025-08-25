// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Security.Cryptography;

namespace EndPointEcommerce.WebApi.Services;

public interface IQuoteCookieManager
{
    int? GetQuoteIdFromCookie(HttpRequest request);
    void SetQuoteIdCookie(HttpResponse response, int quoteId);
    void DeleteQuoteIdCookie(HttpResponse response);
}

public class QuoteCookieManager : IQuoteCookieManager
{
    private const string COOKIE_NAME = "EndPointEcommerce_QuoteId";
    private const int COOKIE_EXPIRATION_DAYS = 7;

    private readonly IDataProtectorProxy _protector;

    public QuoteCookieManager(IDataProtectorProxy protector)
    {
        _protector = protector;
    }

    public int? GetQuoteIdFromCookie(HttpRequest request)
    {
        var quoteCookie = request.Cookies[COOKIE_NAME];
        if (quoteCookie == null) return null;

        try
        {
            return int.Parse(_protector.Unprotect(quoteCookie));
        }
        catch (CryptographicException)
        {
            return null;
        }
    }

    public void SetQuoteIdCookie(HttpResponse response, int quoteId)
    {
        var options = new CookieOptions()
        {
            Expires = DateTimeOffset.Now.AddDays(COOKIE_EXPIRATION_DAYS),
            SameSite = SameSiteMode.None,
            Secure = true,
            HttpOnly = true
        };

        response.Cookies.Append(COOKIE_NAME, _protector.Protect(quoteId.ToString()), options);
    }

    public void DeleteQuoteIdCookie(HttpResponse response)
    {
        response.Cookies.Delete(COOKIE_NAME);
    }
}