// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Domain.Services;

public interface IProductUpdater
{
    Task<Product> Run(ProductInputPayload payload);
}

public class ProductUpdater : IProductUpdater
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public ProductUpdater(IProductRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["ProductImagesPath"]!;
    }

    public async Task<Product> Run(ProductInputPayload payload)
    {
        var productToUpdate = await _repository.FindByIdAsync(payload.Id) ??
            throw new EntityNotFoundException();

        payload.CopyInto(productToUpdate);

        try
        {
            await HandleImageUpdate(
                payload.UploadedMainImageFile,
                productToUpdate.HasMainImage,
                productToUpdate.MainImage?.FileName,
                async () => await _repository.DeleteMainImage(productToUpdate),
                payload.MainImageFile,
                productImage => productToUpdate.MainImage = productImage
            );

            await HandleImageUpdate(
                payload.UploadedThumbnailImageFile,
                productToUpdate.HasThumbnailImage,
                productToUpdate.ThumbnailImage?.FileName,
                async () => await _repository.DeleteThumbnailImage(productToUpdate),
                payload.ThumbnailImageFile,
                productImage => productToUpdate.ThumbnailImage = productImage
            );

            if (payload.UploadedAdditionalImageFiles)
            {
                foreach (var imageFile in payload.AdditionalImageFiles!)
                {
                    var imageFileName = await SaveImageFile(imageFile);
                    productToUpdate.AdditionalImages.Add(new() { FileName = imageFileName });
                }
            }

            await _repository.UpdateAsync(productToUpdate);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProductExists(productToUpdate.Id))
            {
                throw new EntityNotFoundException();
            }
            else
            {
                throw;
            }
        }

        return productToUpdate;
    }

    private async Task HandleImageUpdate(
        bool newImageIsUploaded,
        bool productHasImageAlready,
        string? existingImageFileName,
        Func<Task> deleteImage,
        IFormFile? newImageFile,
        Action<ProductImage> setNewProductImage
    ) {
        if (newImageIsUploaded)
        {
            if (productHasImageAlready)
            {
                DeleteImageFile(existingImageFileName!);
                await deleteImage.Invoke();
            }

            var imageFileName = await SaveImageFile(newImageFile!);
            setNewProductImage.Invoke(new() { FileName = imageFileName });
        }
    }

    private async Task<bool> ProductExists(int id) =>
        await _repository.ExistsAsync(id);

    private async Task<string> SaveImageFile(IFormFile file) =>
        await _fileService.SaveFile(file, _imagesPath);

    private void DeleteImageFile(string fileName)
    {
        var filePath = Path.Combine(_imagesPath, fileName);
        _fileService.DeleteFile(filePath);
    }
}