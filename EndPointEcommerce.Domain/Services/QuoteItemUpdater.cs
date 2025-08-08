using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using static EndPointEcommerce.Domain.Services.IQuoteItemUpdater;

namespace EndPointEcommerce.Domain.Services;

public interface IQuoteItemUpdater
{
    public class InputPayload
    {
        public int QuoteId { get; set; }
        public int QuoteItemId { get; set; }

        public int Quantity { get; set; }
    }

    Task<QuoteItem?> Run(InputPayload payload);
}

public class QuoteItemUpdater : BaseQuoteService, IQuoteItemUpdater
{
    public QuoteItemUpdater(
        IQuoteRepository quoteRepository,
        IQuoteItemRepository quoteItemRepository,
        IQuoteTaxCalculator quoteTaxCalculator
    ) : base(quoteRepository, quoteItemRepository, quoteTaxCalculator) { }

    public async Task<QuoteItem?> Run(InputPayload payload)
    {
        var quote = await FindOpenQuoteOrThrow(payload.QuoteId);
        var quoteItem = FindQuoteItemOrThrow(quote, payload.QuoteItemId);

        if (payload.Quantity != 0)
        {
            var shouldUpdateTax = ShouldUpdateTax(quoteItem, payload);

            ApplyUpdate(quoteItem, payload);
            await _quoteItemRepository.UpdateAsync(quoteItem);

            if (shouldUpdateTax)
            {
                await UpdateTax(quote);
            }

            return quoteItem;
        }
        else
        {
            quote.Items.Remove(quoteItem);
            await _quoteRepository.UpdateAsync(quote);

            await UpdateTax(quote);

            return null;
        }
    }

    private void ApplyUpdate(QuoteItem quoteItem, InputPayload payload)
    {
        quoteItem.Quantity = payload.Quantity;
    }

    private static bool ShouldUpdateTax(QuoteItem quoteItem, InputPayload payload) =>
        payload.Quantity != quoteItem.Quantity;
}
