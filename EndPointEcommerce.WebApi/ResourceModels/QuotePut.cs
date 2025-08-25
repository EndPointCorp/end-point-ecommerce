// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class QuotePut
{
    [EmailAddress]
    public string? Email { get; set; }

    public int? ShippingAddressId { get; set; }
    public QuoteAddress? ShippingAddress { get; set; }

    public int? BillingAddressId { get; set; }
    public QuoteAddress? BillingAddress { get; set; }

    public string? CouponCode { get; set; }
}
