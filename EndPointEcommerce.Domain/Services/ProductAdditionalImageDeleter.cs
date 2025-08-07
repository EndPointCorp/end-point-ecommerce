using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Domain.Services;

public interface IProductAdditionalImageDeleter
{
    Task<Product> Run(int productId, int additionalImageId);
}

public class ProductAdditionalImageDeleter : IProductAdditionalImageDeleter
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public ProductAdditionalImageDeleter(IProductRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["ProductImagesPath"]!;
    }

    public async Task<Product> Run(int productId, int additionalImageId)
    {
        var productToUpdate = await _repository.FindByIdAsync(productId) ??
            throw new EntityNotFoundException();

        var imageToDelete = productToUpdate.GetAdditionalImageById(additionalImageId) ??
            throw new EntityNotFoundException();

        DeleteImageFile(imageToDelete.FileName);
        await _repository.DeleteAdditionalImage(imageToDelete);

        return productToUpdate;
    }

    private void DeleteImageFile(string fileName)
    {
        var filePath = Path.Combine(_imagesPath, fileName);
        _fileService.DeleteFile(filePath);
    }
}
