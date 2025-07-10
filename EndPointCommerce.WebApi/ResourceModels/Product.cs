using EndPointCommerce.Domain.Services;

namespace EndPointCommerce.WebApi.ResourceModels;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? UrlKey { get; set; }
    public Metadata? Metadata { get; set; }

    public bool IsInStock { get; set; }
    public bool IsDiscounted { get; set; }

    public string Sku { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? DiscountAmount { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal? Weight { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public string? MainImageUrl { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    public IEnumerable<string>? AdditionalImagesUrls { get; set; }

    public static Product FromEntity(Domain.Entities.Product entity, string? imagesUrlPath = null)
    {
        return new() {
            Id = entity.Id,
            Name = entity.Name,

            UrlKey = entity.UrlKey,
            Metadata = Metadata.FromEntity(entity),

            IsInStock = entity.IsInStock,
            IsDiscounted = entity.IsDiscounted,

            Sku = entity.Sku,
            BasePrice = entity.BasePrice,
            DiscountAmount = entity.DiscountAmount,

            DiscountedPrice = entity.DiscountedPrice,

            Description = entity.Description,
            ShortDescription = entity.ShortDescription,
            Weight = entity.Weight,

            CategoryId = entity.CategoryId,
            Category = entity.Category is null ? null : Category.FromEntity(entity.Category),

            MainImageUrl = ImageUrlBuilder.GetImageUrl(entity.MainImage, imagesUrlPath),
            ThumbnailImageUrl = ImageUrlBuilder.GetImageUrl(entity.ThumbnailImage, imagesUrlPath),
            AdditionalImagesUrls = entity.AdditionalImages?.Select(i => ImageUrlBuilder.GetImageUrl(i, imagesUrlPath))!
        };
    }

    public static List<Product> FromListOfEntities(IList<Domain.Entities.Product> entities, string? imagesUrlPath = null) =>
        entities.Select(e => FromEntity(e, imagesUrlPath)).ToList();
}
