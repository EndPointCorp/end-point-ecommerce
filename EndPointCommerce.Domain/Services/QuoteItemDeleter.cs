using EndPointCommerce.Domain.Interfaces;
using static EndPointCommerce.Domain.Services.IQuoteItemDeleter;

namespace EndPointCommerce.Domain.Services;

public interface IQuoteItemDeleter
{
    public class InputPayload
    {
        public int QuoteId { get; set; }
        public int QuoteItemId { get; set; }
    }

    Task Run(InputPayload payload);
}

public class QuoteItemDeleter : BaseQuoteService, IQuoteItemDeleter
{
    public QuoteItemDeleter(
        IQuoteRepository quoteRepository,
        IQuoteItemRepository quoteItemRepository,
        IQuoteTaxCalculator quoteTaxCalculator
    ) : base(quoteRepository, quoteItemRepository, quoteTaxCalculator) { }

    public async Task Run(InputPayload payload)
    {
        var quote = await FindOpenQuoteOrThrow(payload.QuoteId);
        var quoteItem = FindQuoteItemOrThrow(quote, payload.QuoteItemId);

        quote.Items.Remove(quoteItem);
        await _quoteRepository.UpdateAsync(quote);

        await UpdateTax(quote);
    }
}
