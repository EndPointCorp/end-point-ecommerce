using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.WebApi.ResourceModels;

public class Metadata
{
    public string? Title { get; set; }
    public string? Keywords { get; set; }
    public string? Description { get; set; }

    public static Metadata FromEntity(BaseSeoEntity entity)
    {
        return new() {
            Title = entity.MetaTitle,
            Keywords = entity.MetaKeywords,
            Description = entity.MetaDescription
        };
    }
}