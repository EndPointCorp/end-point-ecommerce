using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EndPointEcommerce.Domain.Services;

public interface IQuoteTaxCalculator
{
    Task<Quote> Run(Quote quote);
}

public class QuoteTaxCalculator : IQuoteTaxCalculator
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly ITaxCalculator _taxCalculator;
    private readonly ILogger<QuoteTaxCalculator> _logger;

    public QuoteTaxCalculator(
        IQuoteRepository quoteRepository,
        ITaxCalculator taxCalculator,
        ILogger<QuoteTaxCalculator> logger
    ) {
        _quoteRepository = quoteRepository;
        _taxCalculator = taxCalculator;
        _logger = logger;
    }

    public async Task<Quote> Run(Quote quote)
    {
        quote.Tax = 0.0M;

        if (CanCalculateTax(quote))
        {
            try
            {
                var result = await _taxCalculator.Calculate(quote);
                if (result != null) quote.Tax = result.AmountToCollect;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate tax for quote Id: {QuoteId}", quote.Id);
            }
        }

        await _quoteRepository.UpdateAsync(quote);

        return quote;
    }

    private static bool CanCalculateTax(Quote quote)
    {
        return quote.Subtotal != 0 && quote.Items.Count != 0 && quote.ShippingAddress != null;
    }
}