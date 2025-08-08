using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

public interface IQuoteItem
{
    int Id { get; }

    Product Product { get; }
    int ProductId { get; }

    int Quantity { get; }

    decimal UnitPrice { get; }
    decimal TotalPrice { get; }
    decimal Discount { get; }
    decimal Total { get; }
}