// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class QuoteItemPut
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}