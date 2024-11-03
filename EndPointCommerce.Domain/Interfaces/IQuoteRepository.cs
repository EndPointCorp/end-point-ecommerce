using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Quote repository interface.
/// </summary>
public interface IQuoteRepository : IBaseRepository<Quote>
{
    Task<Quote?> FindOpenByIdAsync(int id);
    Task<Quote?> FindOpenByCustomerIdAsync(int customerId);
    Task<Quote> CreateNewAsync(int? customerId);
}
