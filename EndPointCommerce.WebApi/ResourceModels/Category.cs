using EndPointCommerce.WebApi.Services;

namespace EndPointCommerce.WebApi.ResourceModels;

public class Category
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public string? UrlKey { get; set; }
    public Metadata? Metadata { get; set; }

    public string? MainImageUrl { get; set; }

    public static Category FromEntity(Domain.Entities.Category entity, string? imagesUrlPath = null)
    {
        return new() {
            Id = entity.Id,
            Name = entity.Name,
            UrlKey = entity.UrlKey,
            Metadata = Metadata.FromEntity(entity),

            MainImageUrl = ImageUrlBuilder.GetImageUrl(entity.MainImage, imagesUrlPath),
        };
    }

    public static List<Category> FromListOfEntities(IList<Domain.Entities.Category> entities, string? imagesUrlPath = null) =>
        entities.Select(e => FromEntity(e, imagesUrlPath)).ToList();
}
