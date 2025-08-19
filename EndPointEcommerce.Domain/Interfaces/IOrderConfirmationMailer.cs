// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

public interface IOrderConfirmationMailer
{
    Task SendAsync(Order order);
}