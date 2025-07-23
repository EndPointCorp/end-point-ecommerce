using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly string _imagesUrl;

        public CategoriesController(ICategoryRepository repository, IConfiguration config)
        {
            _repository = repository;
            _imagesUrl = config["CategoryImagesUrl"]!;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceModels.Category>>> GetCategories()
        {
            return ResourceModels.Category.FromListOfEntities(
                await _repository.FetchAllAsync(enabledOnly: true),
                _imagesUrl
            );
        }
    }
}
