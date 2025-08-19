// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services.InputPayloads;
using EndPointEcommerce.Domain.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EndPointEcommerce.AdminPortal.ViewModels;

public class ProductViewModel : Product
{
    [Display(Name = "Main")]
    [FileIsNotEmpty]
    [HasImageFileExtension]
    public IFormFile? MainImageFile { get; set; }

    [Display(Name = "Thumbnail")]
    [FileIsNotEmpty]
    [HasImageFileExtension]
    public IFormFile? ThumbnailImageFile { get; set; }

    [Display(Name = "Additionals")]
    [AllFilesAreNotEmpty]
    [AllFilesHaveImageFileExtension]
    public IEnumerable<IFormFile>? AdditionalImageFiles { get; set; }

    public string MainImageUrl => GetAdditionaImageUrl(MainImage);

    public string ThumbnailImageUrl => GetAdditionaImageUrl(ThumbnailImage);

    public string GetAdditionaImageUrl(ProductImage? image) =>
        image is null ? string.Empty : $"~/product-images/{image.FileName}";

    public SelectList? Categories { get; set; }

    public async Task PopulateCategories(ICategoryRepository categoryRepository)
    {
        var categories = await categoryRepository.FetchAllAsync();
        Categories = new SelectList(categories, "Id", "Name");
    }

    public async Task PopulateImages(IProductRepository productRepository)
    {
        var product = await productRepository.FindByIdAsync(Id);
        if (product is null) return;

        MainImage = product.MainImage;
        ThumbnailImage = product.ThumbnailImage;
        AdditionalImages = product.AdditionalImages;
    }

    public static ProductViewModel CreateDefault()
    {
        return new()
        {
            Name = "",
            Sku = "",
            BasePrice = 0
        };
    }

    public static ProductViewModel FromModel(Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            CategoryId = product.CategoryId,

            IsEnabled = product.IsEnabled,
            IsInStock = product.IsInStock,
            IsDiscounted = product.IsDiscounted,

            BasePrice = product.BasePrice,
            DiscountAmount = product.DiscountAmount,

            Description = product.Description,
            ShortDescription = product.ShortDescription,
            Weight = product.Weight,

            UrlKey = product.UrlKey,
            MetaTitle = product.MetaTitle,
            MetaKeywords = product.MetaKeywords,
            MetaDescription = product.MetaDescription,

            MainImage = product.MainImage,
            ThumbnailImage = product.ThumbnailImage,
            AdditionalImages = product.AdditionalImages
        };

    public ProductInputPayload ToInputPayload() =>
        new()
        {
            Id = Id,
            Name = Name,
            Sku = Sku,
            CategoryId = CategoryId,

            IsEnabled = IsEnabled,
            IsInStock = IsInStock,
            IsDiscounted = IsDiscounted,

            BasePrice = BasePrice,
            DiscountAmount = DiscountAmount,

            Description = Description,
            ShortDescription = ShortDescription,
            Weight = Weight,

            UrlKey = UrlKey,
            MetaTitle = MetaTitle,
            MetaKeywords = MetaKeywords,
            MetaDescription = MetaDescription,

            MainImageFile = MainImageFile,
            ThumbnailImageFile = ThumbnailImageFile,
            AdditionalImageFiles = AdditionalImageFiles
        };
}