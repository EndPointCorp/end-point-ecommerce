using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Payment method repository interface.
/// </summary>
public interface IPaymentMethodRepository : IBaseRepository<PaymentMethod>
{
    Task<IEnumerable<PaymentMethod>> FetchAllAsync();
    Task<PaymentMethod> GetFreeOrder();
    Task<PaymentMethod> GetCreditCard();
}
