using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Domain.Services;

public static class ImageUrlBuilder
{
    public static string? GetImageUrl(Image? image, string? imagesUrl) =>
        (image is null || imagesUrl is null) ? null : $"{imagesUrl}/{image.FileName}";
}