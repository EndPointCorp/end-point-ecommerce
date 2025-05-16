using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.RazorTemplates;
using EndPointCommerce.RazorTemplates.Services;
using EndPointCommerce.RazorTemplates.ViewModels;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Infrastructure.Services;

public class OrderConfirmationMailer : IOrderConfirmationMailer
{
    private readonly IMailer _mailer;
    private readonly IRazorViewRenderer _razorViewRenderer;

    public OrderConfirmationMailer(IMailer mailer, IRazorViewRenderer razorViewRenderer)
    {
        _mailer = mailer;
        _razorViewRenderer = razorViewRenderer;
    }

    public async Task SendAsync(Order order)
    {
        string body = await _razorViewRenderer.Render(
            Templates.OrderConfirmation,
            new OrderConfirmationViewModel()
            {
                Order = order
            }
        );

        await _mailer.SendMailAsync(new() {
            To = order.Customer.Email,
            ToName = order.Customer.FullName,
            Subject = $"End Point Commerce: New Order # {order.Id}",
            Body = body
        });
    }
}
