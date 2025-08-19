// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Http;

namespace EndPointEcommerce.Domain.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(IFormFile file, string directory);
    void DeleteFile(string path);
}
