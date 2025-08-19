// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Security.Principal;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.WebApi.Services;

public interface IQuoteResolver
{
    Task<Quote?> ResolveQuote(IPrincipal principal, HttpRequest request);
}

public class QuoteResolver : IQuoteResolver
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IQuoteCookieManager _quoteCookieManager;
    private readonly ISessionHelper _sessionHelper;

    public QuoteResolver(
        IQuoteRepository quoteRepository,
        IQuoteCookieManager quoteCookieManager,
        ISessionHelper sessionHelper
    ) {
        _quoteRepository = quoteRepository;
        _quoteCookieManager = quoteCookieManager;
        _sessionHelper = sessionHelper;
    }

    public async Task<Quote?> ResolveQuote(IPrincipal principal, HttpRequest request)
    {
        if (_sessionHelper.IsAuthenticated(principal))
        {
            var quoteFromCustomer = await GetQuoteFromCustomer(principal);
            var quoteFromCookie = await GetQuoteFromCookie(request);

            return await ResolveQuoteForSession(principal, quoteFromCustomer, quoteFromCookie);
        }
        else
        {
            return await GetQuoteFromCookie(request);
        }
    }

    private async Task<Quote?> GetQuoteFromCustomer(IPrincipal principal)
    {
        var customerId = await _sessionHelper.GetCustomerId(principal) ??
            throw new EntityNotFoundException();

        return await _quoteRepository.FindOpenByCustomerIdAsync(customerId);
    }

    private async Task<Quote?> GetQuoteFromCookie(HttpRequest request)
    {
        var quoteId = _quoteCookieManager.GetQuoteIdFromCookie(request);
        if (quoteId == null) return null;

        return await _quoteRepository.FindOpenByIdAsync(quoteId.Value);
    }

    // Handles the case when a user starts a quote as a guest and then logs in.
    // Makes sure that, after login, the quote is associated with the customer,
    // and that any items they had added as guests get carried over.
    private async Task<Quote?> ResolveQuoteForSession(
        IPrincipal principal,
        Quote? quoteFromCustomer,
        Quote? quoteFromCookie
    ) {
        if (quoteFromCookie != null)
        {
            // If the logged in user has no quote...
            // Then turn the guest quote into a customer quote and return that.
            if (quoteFromCustomer == null)
            {
                await AssignToCustomer(principal, quoteFromCookie);
                return quoteFromCookie;
            }
            // If the logged in user has a quote and it is different than the one from the cookie...
            // Then copy the guest quote into the customer quote and close the guest quote.
            else if (!quoteFromCustomer.Equals(quoteFromCookie))
            {
                await MergeQuotes(quoteFromCustomer, quoteFromCookie);
                await CloseQuote(quoteFromCookie);

                return quoteFromCustomer;
            }
        }

        return quoteFromCustomer;
    }

    private async Task AssignToCustomer(IPrincipal principal, Quote quoteFromCookie)
    {
        quoteFromCookie.CustomerId = await _sessionHelper.GetCustomerId(principal);
        await _quoteRepository.UpdateAsync(quoteFromCookie);
    }

    private async Task MergeQuotes(Quote quoteFromCustomer, Quote quoteFromCookie)
    {
        quoteFromCustomer.IncludeItemsFrom(quoteFromCookie);
        await _quoteRepository.UpdateAsync(quoteFromCustomer);
    }

    private async Task CloseQuote(Quote quoteFromCookie)
    {
        quoteFromCookie.Close();
        await _quoteRepository.UpdateAsync(quoteFromCookie);
    }
}