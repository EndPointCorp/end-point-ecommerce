// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class QuoteItemPost
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}