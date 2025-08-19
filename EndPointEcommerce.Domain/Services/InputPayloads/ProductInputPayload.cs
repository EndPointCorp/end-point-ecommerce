// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace EndPointEcommerce.Domain.Services.InputPayloads;

public class ProductInputPayload : Product
{
    public IFormFile? MainImageFile { get; set; }
    public IFormFile? ThumbnailImageFile { get; set; }
    public IEnumerable<IFormFile>? AdditionalImageFiles { get; set; }

    public bool UploadedMainImageFile =>
        MainImageFile is not null && MainImageFile.Length > 0;

    public bool UploadedThumbnailImageFile =>
        ThumbnailImageFile is not null && ThumbnailImageFile.Length > 0;

    public bool UploadedAdditionalImageFiles =>
        AdditionalImageFiles is not null && AdditionalImageFiles.Any(f => f.Length > 0);

    public void CopyInto(Product product)
    {
        product.Name = Name;
        product.Sku = Sku;
        product.CategoryId = CategoryId;

        product.IsEnabled = IsEnabled;
        product.IsInStock = IsInStock;
        product.IsDiscounted = IsDiscounted;

        product.BasePrice = BasePrice;
        product.DiscountAmount = DiscountAmount;

        product.Description = Description;
        product.ShortDescription = ShortDescription;
        product.Weight = Weight;

        product.UrlKey = UrlKey;
        product.MetaTitle = MetaTitle;
        product.MetaKeywords = MetaKeywords;
        product.MetaDescription = MetaDescription;
    }
}
