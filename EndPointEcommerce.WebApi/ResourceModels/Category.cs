// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Services;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? UrlKey { get; set; }
    public Metadata? Metadata { get; set; }

    public string? MainImageUrl { get; set; }

    public static Category FromEntity(Domain.Entities.Category entity, string? imagesUrl = null)
    {
        return new() {
            Id = entity.Id,
            Name = entity.Name,
            UrlKey = entity.UrlKey,
            Metadata = Metadata.FromEntity(entity),

            MainImageUrl = ImageUrlBuilder.GetImageUrl(entity.MainImage, imagesUrl),
        };
    }

    public static List<Category> FromListOfEntities(IList<Domain.Entities.Category> entities, string? imagesUrl = null) =>
        entities.Select(e => FromEntity(e, imagesUrl)).ToList();
}
