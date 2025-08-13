using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EndPointEcommerce.Domain.Services;

public interface IProductThumbnailImageDeleter
{
    Task<Product> Run(int productId);
}

public class ProductThumbnailImageDeleter : BaseProductImageDeleter, IProductThumbnailImageDeleter
{
    public ProductThumbnailImageDeleter(
        IProductRepository repository, IFileService fileService, IConfiguration config
    ) : base(repository, fileService, config) { }

    protected override bool HasImage(Product product) => product.HasThumbnailImage;
    protected override string GetImageFileName(Product product) => product.ThumbnailImage!.FileName;
    protected override async Task DeleteImage(Product product) => await _repository.DeleteThumbnailImage(product);
}
