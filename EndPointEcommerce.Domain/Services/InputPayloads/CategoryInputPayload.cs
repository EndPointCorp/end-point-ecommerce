// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace EndPointEcommerce.Domain.Services.InputPayloads;

public class CategoryInputPayload : Category
{
    public IFormFile? MainImageFile { get; set; }

    public bool UploadedMainImageFile =>
        MainImageFile is not null && MainImageFile.Length > 0;

    public void CopyInto(Category category)
    {
        category.Name = Name;
        category.IsEnabled = IsEnabled;
        category.UrlKey = UrlKey;
        category.MetaTitle = MetaTitle;
        category.MetaKeywords = MetaKeywords;
        category.MetaDescription = MetaDescription;
    }
}
