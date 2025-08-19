// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.WebStore.Api;

public record Order(
    int Id,
    string Guid,
    string Status,
    string TrackingNumber,
    decimal Subtotal,
    decimal Discount,
    decimal Tax,
    decimal Total,
    List<QuoteItem> Items,
    Address ShippingAddress,
    Address BillingAddress,
    DateTime DateCreated
);
