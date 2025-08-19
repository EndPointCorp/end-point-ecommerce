// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Services;

public static class ImageUrlBuilder
{
    public static string? GetImageUrl(Image? image, string? imagesUrl) =>
        (image is null || imagesUrl is null) ? null : $"{imagesUrl}/{image.FileName}";
}