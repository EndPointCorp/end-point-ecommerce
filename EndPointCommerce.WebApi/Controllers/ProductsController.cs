using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly string _imagesUrl;

        public ProductsController(IProductRepository repository, IConfiguration config)
        {
            _repository = repository;
            _imagesUrl = config["ProductImagesUrl"]!;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceModels.Product>>> GetProducts()
        {
            return ResourceModels.Product.FromListOfEntities(
                await _repository.FetchAllAsync(enabledOnly: true),
                _imagesUrl
            );
        }

        // GET: api/Products/CategoryId/{categoryId}
        [HttpGet("CategoryId/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ResourceModels.Product>>> GetProductsByCategoryId(int categoryId)
        {
            return ResourceModels.Product.FromListOfEntities(
                await _repository.FetchAllByCategoryIdAsync(categoryId, enabledOnly: true),
                _imagesUrl
            );
        }

        // GET: api/Products/CategoryUrlKey/{categoryUrlKey}
        [HttpGet("CategoryUrlKey/{categoryUrlKey}")]
        public async Task<ActionResult<IEnumerable<ResourceModels.Product>>> GetProductsByCategoryUrlKey(string categoryUrlKey)
        {
            return ResourceModels.Product.FromListOfEntities(
                await _repository.FetchAllByCategoryUrlKeyAsync(categoryUrlKey, enabledOnly: true),
                _imagesUrl
            );
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceModels.Product>> GetProduct(int id)
        {
            var product = await _repository.FindByIdAsync(id, enabledOnly: true);

            if (product == null) return NotFound();

            return ResourceModels.Product.FromEntity(product, _imagesUrl);
        }

        // GET: api/Products/UrlKey/{urlKey}
        [HttpGet("UrlKey/{urlKey}")]
        public async Task<ActionResult<ResourceModels.Product>> GetProductByUrlKey(string urlKey)
        {
            var product = await _repository.FindByUrlKeyAsync(urlKey, enabledOnly: true);

            if (product == null) return NotFound();

            return ResourceModels.Product.FromEntity(product, _imagesUrl);
        }
    }
}
