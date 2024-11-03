using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EndPointCommerce.Domain.Services;

public interface IProductMainImageDeleter
{
    Task<Product> Run(int productId);
}

public class ProductMainImageDeleter : BaseProductImageDeleter, IProductMainImageDeleter
{
    public ProductMainImageDeleter(
        IProductRepository repository, IFileService fileService, IConfiguration config
    ) : base(repository, fileService, config) { }

    protected override bool HasImage(Product product) => product.HasMainImage;
    protected override string GetImageFileName(Product product) => product.MainImage!.FileName;
    protected override async Task DeleteImage(Product product) => await _repository.DeleteMainImage(product);
}
