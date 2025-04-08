using EndPointCommerce.WebStore.Api;

namespace EndPointCommerce.WebStore.ViewModels;

public record class ProductCardViewModel(
    Product Product,
    Dictionary<string, string> AddToCartRouteData
);
