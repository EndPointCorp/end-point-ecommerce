using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Domain.Services;

public abstract class BaseProductImageDeleter
{
    protected readonly IProductRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public BaseProductImageDeleter(IProductRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["ProductImagesPath"]!;
    }

    protected abstract bool HasImage(Product product);
    protected abstract string GetImageFileName(Product product);
    protected abstract Task DeleteImage(Product product);

    public async Task<Product> Run(int productId)
    {
        var productToUpdate = await _repository.FindByIdAsync(productId) ??
            throw new EntityNotFoundException();

        if (HasImage(productToUpdate))
        {
            DeleteImageFile(GetImageFileName(productToUpdate));
            await DeleteImage(productToUpdate);
        }

        return productToUpdate;
    }

    protected void DeleteImageFile(string fileName)
    {
        var filePath = Path.Combine(_imagesPath, fileName);
        _fileService.DeleteFile(filePath);
    }
}
