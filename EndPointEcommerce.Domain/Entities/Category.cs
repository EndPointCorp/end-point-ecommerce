// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EndPointEcommerce.Domain.Validation;
using Microsoft.EntityFrameworkCore;

namespace EndPointEcommerce.Domain.Entities;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(UrlKey), IsUnique = true)]
public class Category : BaseSeoEntity
{
    [UniqueCategoryName]
    public required string Name { get; set; }

    [UniqueCategoryUrlKey]
    public override string? UrlKey { get; set; }

    [Display(Name = "Enabled")]
    public bool IsEnabled { get; set; } = true;

    public ICollection<Product> Products { get; } = [];

    [ForeignKey("MainImage")]
    public int? MainImageId { get; set; }
    public CategoryImage? MainImage { get; set; }

    public bool HasMainImage => MainImage is not null;
}