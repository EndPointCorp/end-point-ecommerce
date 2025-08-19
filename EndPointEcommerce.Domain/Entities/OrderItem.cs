// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity that holds the item information for a given order.
/// </summary>
public class OrderItem : BaseEntity, IQuoteItem
{
    public Order Order { get; set; } = default!;
    [Required]
    public int OrderId { get; set; }

    public QuoteItem? QuoteItem { get; set; }
    public int? QuoteItemId { get; set; }

    public Product Product { get; set; } = default!;
    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    [Required]
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }
    [Required]
    public decimal Discount { get; set; }
    [Required]
    public decimal Total { get; set; }

    public static OrderItem Build(QuoteItem item) =>
        new OrderItem
        {
            QuoteItemId = item.Id,
            ProductId = item.ProductId,
            Product = item.Product,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            Discount = item.Discount,
            Total = item.Total
        };
}