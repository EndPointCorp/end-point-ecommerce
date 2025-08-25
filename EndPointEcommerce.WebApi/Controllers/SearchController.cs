// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using EndPointEcommerce.Domain.Interfaces;
using static EndPointEcommerce.Domain.Interfaces.IProductRepository;

namespace EndPointEcommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly string _imagesUrl;

        public SearchController(IProductRepository repository, IConfiguration config)
        {
            _repository = repository;
            _imagesUrl = config["ProductImagesUrl"]!;
        }

        // GET: api/Search/Products/{query}
        [HttpGet("Products/{query}")]
        public async Task<ActionResult<IEnumerable<ResourceModels.Product>>> GetSearchProducts(string query)
        {
            return ResourceModels.Product.FromListOfEntities(
                await _repository.FetchAllBySearchQueryAsync(query), _imagesUrl
            );
        }

        // GET: api/Search/Suggestions/Products/{query}
        [HttpGet("Suggestions/Products/{query}")]
        public async Task<ActionResult<IEnumerable<SearchSuggestion>>> GetSearchSuggestionsProducts(string query)
        {
            return Ok(await _repository.FetchAllSuggestionsBySearchQueryAsync(query));
        }
    }
}
