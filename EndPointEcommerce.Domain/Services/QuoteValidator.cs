// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.Domain.Services;

public interface IQuoteValidator
{
    Task<Quote> Run(int quoteId);
}

public class QuoteValidator : BaseQuoteService, IQuoteValidator
{
    public QuoteValidator(
        IQuoteRepository quoteRepository,
        IQuoteItemRepository quoteItemRepository,
        IQuoteTaxCalculator quoteTaxCalculator
    ) : base(quoteRepository, quoteItemRepository, quoteTaxCalculator) { }

    public async Task<Quote> Run(int quoteId)
    {
        var quote = await FindOpenQuoteOrThrow(quoteId);
        ThrowIfNotValid(quote);

        return quote;
    }

    private static void ThrowIfNotValid(Quote quote)
    {
        var (isValid, results) = quote.Validate();

        if (!isValid)
            throw new DomainValidationException("The quote is not valid.", results);
    }
}
