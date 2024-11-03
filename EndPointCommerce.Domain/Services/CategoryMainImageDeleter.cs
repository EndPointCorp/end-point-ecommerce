using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Domain.Services;

public interface ICategoryMainImageDeleter
{
    Task<Category> Run(int categoryId);
}

public class CategoryMainImageDeleter : ICategoryMainImageDeleter
{
    private readonly ICategoryRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public CategoryMainImageDeleter(ICategoryRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["CategoryImagesPath"]!;
    }

    public async Task<Category> Run(int categoryId)
    {
        var categoryToUpdate = await _repository.FindByIdAsync(categoryId) ??
            throw new EntityNotFoundException();

        if (categoryToUpdate.HasMainImage)
        {
            DeleteImageFile(categoryToUpdate.MainImage!.FileName);
            await _repository.DeleteMainImage(categoryToUpdate);
        }

        return categoryToUpdate;
    }

    private void DeleteImageFile(string fileName)
    {
        var filePath = Path.Combine(_imagesPath, fileName);
        _fileService.DeleteFile(filePath);
    }
}