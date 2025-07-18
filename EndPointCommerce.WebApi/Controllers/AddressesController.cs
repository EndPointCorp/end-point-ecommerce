using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.WebApi.Services;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressRepository _repository;
        private readonly ISessionHelper _sessionHelper;

        public AddressesController(
            IAddressRepository repository,
            ISessionHelper sessionHelper
        ) {
            _repository = repository;
            _sessionHelper = sessionHelper;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceModels.Address>>> GetAddresses()
        {
            var customerId = await _sessionHelper.GetCustomerId(User);
            if (customerId == null) return NotFound();

            return ResourceModels.Address.FromListOfEntities(
                await _repository.FetchAllByCustomerIdAsync(customerId.Value)
            );
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<ResourceModels.Address>> PostAddress([FromBody] ResourceModels.Address payload)
        {
            var customerId = await _sessionHelper.GetCustomerId(User);
            if (customerId == null) return NotFound();

            var address = payload.ToEntity();
            address.CustomerId = customerId;

            // Create the address
            await _repository.AddAsync(address);
            address = await _repository.FindByIdAsync(address.Id);

            return ResourceModels.Address.FromEntity(address!);
        }

        // PUT: api/Addresses/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResourceModels.Address>> PutAddress(
            int id,
            [FromBody] ResourceModels.Address payload
        ) {
            var customerId = await _sessionHelper.GetCustomerId(User);
            if (customerId == null) return NotFound();

            var address = await _repository.FindByIdAsync(id);
            if (address == null) return NotFound();

            if (address.CustomerId != customerId) return NotFound();

            // Update the address
            address = payload.UpdateEntity(address);

            await _repository.UpdateAsync(address);
            address = await _repository.FindByIdAsync(address.Id);

            return ResourceModels.Address.FromEntity(address!);
        }

        // DELETE: api/Addresses
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAddress(int id)
        {
            var customerId = await _sessionHelper.GetCustomerId(User);
            if (customerId == null) return NotFound();

            var address = await _repository.FindByIdAsync(id);
            if (address == null) return NotFound();

            if (address.CustomerId != customerId) return NotFound();

            // Delete the address
            await _repository.DeleteAsync(address);

            return NoContent();
        }
    }
}
