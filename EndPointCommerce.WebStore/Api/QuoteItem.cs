namespace EndPointCommerce.WebStore.Api;

public record QuoteItem(int Id, int ProductId, int Quantity, decimal Total, Product Product);
