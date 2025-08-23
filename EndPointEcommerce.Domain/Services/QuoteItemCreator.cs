using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using static EndPointEcommerce.Domain.Services.IQuoteItemCreator;

namespace EndPointEcommerce.Domain.Services;

public interface IQuoteItemCreator
{
    public class InputPayload
    {
        public int? QuoteId { get; set; }
        public int? CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    Task<QuoteItem> Run(InputPayload payload);
}

public class QuoteItemCreator : BaseQuoteService, IQuoteItemCreator
{
    private readonly IProductRepository _productRepository;

    public QuoteItemCreator(
        IProductRepository productRepository,
        IQuoteRepository quoteRepository,
        IQuoteTaxCalculator quoteTaxCalculator,
        IQuoteItemRepository quoteItemRepository
    ) : base(quoteRepository, quoteItemRepository, quoteTaxCalculator)
    {
        _productRepository = productRepository;
    }

    public async Task<QuoteItem> Run(InputPayload payload)
    {
        var product = await FindProductOrThrow(payload.ProductId);
        var quote = await FindOrCreateQuoteOrThrow(payload.QuoteId, payload.CustomerId);

        var matchingQuoteItem = quote.GetItemBy(productId: payload.ProductId);

        var result = matchingQuoteItem == null ?
            await CreateItem(payload, product, quote) :
            await UpdateItem(payload, matchingQuoteItem);

        await UpdateTax(quote);

        return result;
    }

    private async Task<QuoteItem> CreateItem(InputPayload payload, Product product, Quote quote)
    {
        var quoteItem = QuoteItem.Build(quote, product, payload.Quantity);

        quote.Items.Add(quoteItem);

        await _quoteRepository.UpdateAsync(quote);

        return quoteItem;
    }

    private async Task<QuoteItem> UpdateItem(InputPayload payload, QuoteItem matchingQuoteItem)
    {
        if (payload.Quantity == 0) return matchingQuoteItem;

        matchingQuoteItem.Quantity += payload.Quantity;

        await _quoteItemRepository.UpdateAsync(matchingQuoteItem);

        return matchingQuoteItem;
    }

    private async Task<Product> FindProductOrThrow(int productId) =>
        await _productRepository.FindByIdAsync(productId) ??
            throw new EntityNotFoundException("Product not found");
}
