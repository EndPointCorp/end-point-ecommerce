// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.Domain.Entities;

public class ProductImage : Image
{
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
}