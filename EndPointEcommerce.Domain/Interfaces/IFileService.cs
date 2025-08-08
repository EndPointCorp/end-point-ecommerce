using Microsoft.AspNetCore.Http;

namespace EndPointEcommerce.Domain.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(IFormFile file, string directory);
    void DeleteFile(string path);
}
