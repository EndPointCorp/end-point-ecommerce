// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Domain.Entities;

[Index(nameof(Name), IsUnique = true)]
public class SiteContent: BaseEntity
{
    public required string Name { get; set; }
    public string? Content { get; set; }
}
