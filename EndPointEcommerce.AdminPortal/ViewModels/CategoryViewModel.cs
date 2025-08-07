using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Services.InputPayloads;
using EndPointEcommerce.Domain.Validation;

namespace EndPointEcommerce.AdminPortal.ViewModels;

public class CategoryViewModel : Category
{
    [FileIsNotEmpty]
    [HasImageFileExtension]
    public IFormFile? MainImageFile { get; set; }

    public string MainImageUrl =>
        MainImage is null ? string.Empty : $"~/category-images/{MainImage.FileName}";

    public static CategoryViewModel FromModel(Category category) =>
        new()
        {
            Id = category.Id,
            Name = category.Name,
            IsEnabled = category.IsEnabled,
            UrlKey = category.UrlKey,
            MetaTitle = category.MetaTitle,
            MetaKeywords = category.MetaKeywords,
            MetaDescription = category.MetaDescription,

            MainImage = category.MainImage
        };

    public CategoryInputPayload ToInputPayload() =>
        new()
        {
            Id = Id,
            Name = Name,
            IsEnabled = IsEnabled,
            UrlKey = UrlKey,
            MetaTitle = MetaTitle,
            MetaKeywords = MetaKeywords,
            MetaDescription = MetaDescription,

            MainImageFile = MainImageFile
        };
}