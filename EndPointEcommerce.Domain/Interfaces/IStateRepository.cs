using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// State repository interface.
/// </summary>
public interface IStateRepository : IBaseRepository<State>
{
    public Task<IEnumerable<State>> FetchAllAsync();
}
