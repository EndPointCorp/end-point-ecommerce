using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Interfaces;

public interface IOrderConfirmationMailer
{
    Task SendAsync(Order order);
}