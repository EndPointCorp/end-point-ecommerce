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
