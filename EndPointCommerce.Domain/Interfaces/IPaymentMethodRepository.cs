using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

/// <summary>
/// Payment method repository interface.
/// </summary>
public interface IPaymentMethodRepository : IBaseRepository<PaymentMethod>
{
    Task<IEnumerable<PaymentMethod>> FetchAllAsync();
    Task<PaymentMethod> GetFreeOrder();
    Task<PaymentMethod> GetCreditCard();
}
