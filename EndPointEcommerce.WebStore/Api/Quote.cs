// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.WebStore.Api;

public record Quote(
    int Id,
    decimal Subtotal,
    decimal Total,
    List<QuoteItem> Items,
    string? Email,
    Address? ShippingAddress,
    Address? BillingAddress
);
