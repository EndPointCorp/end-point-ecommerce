using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EndPointCommerce.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace EndPointCommerce.Domain.Entities;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(UrlKey), IsUnique = true)]
public class Product: BaseSeoEntity
{
    [UniqueProductName]
    public required string Name { get; set; }

    [UniqueProductUrlKey]
    public override string? UrlKey { get; set; }

    [Display(Name = "SKU")]
    public required string Sku { get; set; }

    [Display(Name = "Enabled")]
    public bool IsEnabled { get; set; } = true;
    [Display(Name = "Is In Stock")]
    public bool IsInStock { get; set; } = true;

    [Display(Name = "Is Discounted")]
    public bool IsDiscounted { get; set; } = false;

    [Display(Name = "Base Price")]
    public required decimal BasePrice { get; set; }
    [Display(Name = "Amount to reduce 'Base Price' by when the product is discounted")]
    public decimal? DiscountAmount { get; set; }

    public string? Description { get; set; }
    [Display(Name = "Short Description")]
    public string? ShortDescription { get; set; }
    [Display(Name = "Weight (lbs)")]
    public decimal? Weight { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    [ForeignKey("MainImage")]
    public int? MainImageId { get; set; }
    public ProductImage? MainImage { get; set; }

    [ForeignKey("ThumbnailImage")]
    public int? ThumbnailImageId { get; set; }
    public ProductImage? ThumbnailImage { get; set; }

    [InverseProperty("Product")]
    public ICollection<ProductImage> AdditionalImages { get; set; } = [];

    [JsonIgnore]
    public NpgsqlTsVector? SearchVector { get; set; }

    public bool HasMainImage => MainImage is not null;
    public bool HasThumbnailImage => ThumbnailImage is not null;

    public ProductImage? GetAdditionalImageById(int additionalImageId) =>
        AdditionalImages.FirstOrDefault(i => i.Id == additionalImageId);

    public decimal? DiscountedPrice => BasePrice - DiscountAmount;

    public decimal GetActualPrice()
    {
        var price = BasePrice;

        if (IsDiscounted) price -= DiscountAmount ?? 0;

        return Math.Max(price, 0);
    }
}