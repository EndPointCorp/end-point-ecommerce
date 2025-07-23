using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Address repository interface.
/// </summary>
public interface IAddressRepository : IBaseRepository<Address>
{
    public Task<IList<Address>> FetchAllByCustomerIdAsync(int customerId);
}
