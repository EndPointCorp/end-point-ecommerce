using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services.InputPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Domain.Services;

public interface ICategoryCreator
{
    Task<Category> Run(CategoryInputPayload payload);
}

public class CategoryCreator : ICategoryCreator
{
    private readonly ICategoryRepository _repository;
    private readonly IFileService _fileService;
    private readonly string _imagesPath;

    public CategoryCreator(ICategoryRepository repository, IFileService fileService, IConfiguration config)
    {
        _repository = repository;
        _fileService = fileService;
        _imagesPath = config["CategoryImagesPath"]!;
    }

    public async Task<Category> Run(CategoryInputPayload payload)
    {
        var categoryToCreate = payload;

        if (payload.UploadedMainImageFile)
        {
            var mainImageFileName = await SaveImageFile(payload.MainImageFile!);
            categoryToCreate.MainImage = new() { FileName = mainImageFileName };
        }

        await _repository.AddAsync(categoryToCreate);

        return categoryToCreate;
    }

    private async Task<string> SaveImageFile(IFormFile file) =>
        await _fileService.SaveFile(file, _imagesPath);
}