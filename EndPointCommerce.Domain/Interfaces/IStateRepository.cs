using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// State repository interface.
/// </summary>
public interface IStateRepository : IBaseRepository<State>
{
    public Task<IEnumerable<State>> FetchAllAsync();
}
