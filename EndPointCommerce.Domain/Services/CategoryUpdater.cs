using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Domain.Services;

public interface ICategoryUpdater
{
    Task<Category> Run(CategoryInputPayload payload);
}

public class CategoryUpdater : ICategoryUpdater
{
    private readonly ICategoryRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public CategoryUpdater(ICategoryRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["CategoryImagesPath"]!;
    }

    public async Task<Category> Run(CategoryInputPayload payload)
    {
        var categoryToUpdate = await _repository.FindByIdAsync(payload.Id) ??
            throw new EntityNotFoundException();

        payload.CopyInto(categoryToUpdate);

        try
        {
            if (payload.UploadedMainImageFile)
            {
                if (categoryToUpdate.HasMainImage)
                {
                    DeleteImageFile(categoryToUpdate.MainImage!.FileName);
                    await _repository.DeleteMainImage(categoryToUpdate);
                }

                var mainImageFileName = await SaveImageFile(payload.MainImageFile!);
                categoryToUpdate.MainImage = new() { FileName = mainImageFileName };
            }

            await _repository.UpdateAsync(categoryToUpdate);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CategoryExists(categoryToUpdate.Id))
            {
                throw new EntityNotFoundException();
            }
            else
            {
                throw;
            }
        }

        return categoryToUpdate;
    }

    private async Task<bool> CategoryExists(int id) =>
        await _repository.ExistsAsync(id);

    private async Task<string> SaveImageFile(IFormFile file) =>
        await _fileService.SaveFile(file, _imagesPath);

    private void DeleteImageFile(string fileName)
    {
        var filePath = Path.Combine(_imagesPath, fileName);
        _fileService.DeleteFile(filePath);
    }
}