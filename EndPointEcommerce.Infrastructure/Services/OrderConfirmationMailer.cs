// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.RazorTemplates.Services;
using EndPointEcommerce.RazorTemplates.ViewModels;
using EndPointEcommerce.RazorTemplates.Views;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Infrastructure.Services;

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
            Subject = $"End Point Ecommerce: New Order # {order.Id}",
            Body = body
        });
    }
}
