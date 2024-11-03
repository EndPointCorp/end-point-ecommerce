using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using static EndPointCommerce.Domain.Services.IQuoteItemUpdater;

namespace EndPointCommerce.Domain.Services;

public interface IQuoteItemUpdater
{
    public class InputPayload
    {
        public int QuoteId { get; set; }
        public int QuoteItemId { get; set; }

        public int? Quantity { get; set; }
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
        if (payload.Quantity != null) quoteItem.Quantity = payload.Quantity.Value;
    }

    private static bool ShouldUpdateTax(QuoteItem quoteItem, InputPayload payload) =>
        payload.Quantity != quoteItem.Quantity;
}
