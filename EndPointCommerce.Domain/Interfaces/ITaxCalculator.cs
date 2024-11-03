using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Tax service interface.
/// </summary>
public interface ITaxCalculator
{
    public class TaxResponse
    {
        public decimal AmountToCollect { get; set; }
    }

    Task<TaxResponse?> Calculate(Quote quote);
}
