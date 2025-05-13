using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _repository;

        public CountriesController(ICountryRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Countries/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceModels.Country>>> GetCountries()
        {
            return ResourceModels.Country.FromListOfEntities(
                (await _repository.FetchAllEnabledAsync()).ToList()
            );
        }
    }
}
