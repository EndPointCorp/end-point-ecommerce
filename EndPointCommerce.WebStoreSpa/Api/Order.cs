namespace EndPointCommerce.WebStoreSpa.Api;

public record Order(
    int Id,
    string Status,
    string TrackingNumber,
    decimal Total,
    List<QuoteItem> Items,
    Address ShippingAddress,
    Address BillingAddress
);
