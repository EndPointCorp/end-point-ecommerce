// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.Domain.Services;

public abstract class BaseQuoteService
{
    protected readonly IQuoteRepository _quoteRepository;
    protected readonly IQuoteItemRepository _quoteItemRepository;
    private readonly IQuoteTaxCalculator _quoteTaxCalculator;

    protected BaseQuoteService(
        IQuoteRepository quoteRepository,
        IQuoteItemRepository quoteItemRepository,
        IQuoteTaxCalculator quoteTaxCalculator
    ) {
        _quoteRepository = quoteRepository;
        _quoteItemRepository = quoteItemRepository;
        _quoteTaxCalculator = quoteTaxCalculator;
    }

    protected async Task<Quote> FindOpenQuoteOrThrow(int quoteId) =>
        await _quoteRepository.FindOpenByIdAsync(quoteId) ??
            throw new EntityNotFoundException("Quote not found");

    protected static QuoteItem FindQuoteItemOrThrow(Quote quote, int quoteItemId) =>
        quote.GetItemById(quoteItemId) ??
            throw new EntityNotFoundException("Quote item not found");

    protected async Task<Quote> FindOrCreateQuoteOrThrow(int? quoteId, int? customerId)
    {
        // Create the new quote if no ID is provided
        if (quoteId == null)
            return await _quoteRepository.CreateNewAsync(customerId);
        else
            return await _quoteRepository.FindOpenByIdAsync(quoteId.Value) ??
                throw new EntityNotFoundException("Quote not found");
    }

    protected async Task UpdateTax(Quote quote) => await _quoteTaxCalculator.Run(quote);
}