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
    private readonly string _orderDetailsUrlPath;
    private readonly string _productImagesUrlPath;

    public OrderConfirmationMailer(IMailer mailer, IRazorViewRenderer razorViewRenderer, IConfiguration config)
    {
        _mailer = mailer;
        _razorViewRenderer = razorViewRenderer;
        _orderDetailsUrlPath = config["OrderDetailsUrlPath"]!;
        _productImagesUrlPath = config["ProductImagesUrlPath"]!;
    }

    public async Task SendAsync(Order order)
    {
        string body = await _razorViewRenderer.Render(
            Templates.OrderConfirmation,
            new OrderConfirmationViewModel()
            {
                Order = order,
                OrderDetailsUrlPath = _orderDetailsUrlPath,
                ProductImagesUrlPath = _productImagesUrlPath
            }
        );

        await _mailer.SendMailAsync(new()
        {
            To = order.Customer.Email,
            ToName = order.Customer.FullName,
            Subject = $"End Point Commerce: New Order # {order.Id}",
            Body = body
        });
    }
}
