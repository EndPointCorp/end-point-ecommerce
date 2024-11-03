using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for order items.
/// </summary>
public class OrderItemViewModel : OrderItem
{
    public IEnumerable<SelectListItem>? Products { get; set; }

    public OrderItem ToModel()
    {
        return new OrderItem() {
            Id = Id,
            OrderId = OrderId,
            QuoteItemId = QuoteItemId,
            ProductId = ProductId,
            UnitPrice = UnitPrice,
            TotalPrice = TotalPrice,
            Discount = Discount,
            Total = Total
        };
    }

    public async Task FillProducts(IProductRepository productRepository)
    {
        var products = await productRepository.FetchAllAsync();
        Products = products
            .Select(x => new SelectListItem()
            {
                Text = $"{x.Name} ({x.Sku})",
                Value = x.Id.ToString()
            })
            .ToList();
    }

    public static async Task<OrderItemViewModel> CreateDefault(int orderId, IOrderRepository orderRepository,
        IProductRepository productRepository, IAddressRepository addressRepository)
    {
        var orderItemViewModel = new OrderItemViewModel() {
            OrderId = orderId,
            ProductId = 0,
            UnitPrice = 0.0M,
            TotalPrice = 0.0M,
            Discount = 0.0M,
            Total = 0.0M,
        };
        await orderItemViewModel.FillProducts(productRepository);
        return orderItemViewModel;
    }

    public static async Task<OrderItemViewModel> FromModel(OrderItem model, IOrderRepository orderRepository,
        IProductRepository productRepository, IAddressRepository addressRepository)
    {
        var orderItemViewModel = await CreateDefault(model.OrderId, orderRepository, productRepository, addressRepository);
        orderItemViewModel.Id = model.Id;
        orderItemViewModel.OrderId = model.OrderId;
        orderItemViewModel.QuoteItemId = model.QuoteItemId;
        orderItemViewModel.ProductId = model.ProductId;
        orderItemViewModel.UnitPrice = model.UnitPrice;
        orderItemViewModel.TotalPrice = model.TotalPrice;
        orderItemViewModel.Discount = model.Discount;
        orderItemViewModel.Total = model.Total;
        return orderItemViewModel;
    }
}