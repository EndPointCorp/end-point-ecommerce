// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly IStateRepository _repository;

        public StatesController(IStateRepository repository)
        {
            _repository = repository;
        }

        // GET: api/States/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceModels.State>>> GetStates()
        {
            return ResourceModels.State.FromListOfEntities(
                (await _repository.FetchAllAsync()).ToList()
            );
        }
    }
}
