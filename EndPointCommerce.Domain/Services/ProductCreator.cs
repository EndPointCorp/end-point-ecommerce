using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Domain.Services;

public interface IProductCreator
{
    Task<Product> Run(ProductInputPayload payload);
}

public class ProductCreator : IProductCreator
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public ProductCreator(IProductRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["ProductImagesPath"]!;
    }

    public async Task<Product> Run(ProductInputPayload payload)
    {
        var productToCreate = payload;

        if (payload.UploadedMainImageFile)
        {
            var mainImageFileName = await SaveImageFile(payload.MainImageFile!);
            productToCreate.MainImage = new() { FileName = mainImageFileName };
        }

        if (payload.UploadedThumbnailImageFile)
        {
            var thumbnailImageFileName = await SaveImageFile(payload.ThumbnailImageFile!);
            productToCreate.ThumbnailImage = new() { FileName = thumbnailImageFileName };
        }

        if (payload.UploadedAdditionalImageFiles)
        {
            foreach (var imageFile in payload.AdditionalImageFiles!)
            {
                var imageFileName = await SaveImageFile(imageFile);
                productToCreate.AdditionalImages.Add(new() { FileName = imageFileName });
            }
        }

        await _repository.AddAsync(productToCreate);

        return productToCreate;
    }

    private async Task<string> SaveImageFile(IFormFile file) =>
        await _fileService.SaveFile(file, _imagesPath);
}