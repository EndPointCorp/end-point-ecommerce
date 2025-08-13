using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Customer repository interface.
/// </summary>
public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<IEnumerable<Customer>> FetchAllAsync();
    Task<Customer?> FindByEmailAsync(string email);
    Task<int> GetMonthCustomersCount();
}
