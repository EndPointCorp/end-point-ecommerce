using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

public interface IOrderConfirmationMailer
{
    Task SendAsync(Order order);
}