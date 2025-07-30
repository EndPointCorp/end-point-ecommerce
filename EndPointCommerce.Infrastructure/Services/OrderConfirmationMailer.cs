using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.RazorTemplates.Services;
using EndPointCommerce.RazorTemplates.ViewModels;
using EndPointCommerce.RazorTemplates.Views;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Infrastructure.Services;

public class OrderConfirmationMailer : IOrderConfirmationMailer
{
    private readonly IMailer _mailer;
    private readonly IRazorViewRenderer _razorViewRenderer;
    private readonly string _orderDetailsUrl;
    private readonly string _productImagesUrl;

    public OrderConfirmationMailer(IMailer mailer, IRazorViewRenderer razorViewRenderer, IConfiguration config)
    {
        _mailer = mailer;
        _razorViewRenderer = razorViewRenderer;
        _orderDetailsUrl = config["OrderDetailsUrl"]!;
        _productImagesUrl = config["ProductImagesUrl"]!;
    }

    public async Task SendAsync(Order order)
    {
        string body = await _razorViewRenderer.Render<OrderConfirmation, OrderConfirmationViewModel>(
            new OrderConfirmationViewModel()
            {
                Order = order,
                OrderDetailsUrl = _orderDetailsUrl,
                ProductImagesUrl = _productImagesUrl
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
