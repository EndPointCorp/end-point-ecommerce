using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.Addresses
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICustomerRepository _customerRepository;

        public EditModel(IAddressRepository addressRepository, IStateRepository stateRepository, 
            ICustomerRepository customerRepository)
        {
            _addressRepository = addressRepository;
            _stateRepository = stateRepository;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public AddressViewModel Address { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _addressRepository.FindByIdAsync(id.Value);
            if (address == null)
            {
                return NotFound();
            }
            Address = await AddressViewModel.FromModel(address, _stateRepository, _customerRepository);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index", new { customerId = Address.CustomerId }));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(Page);
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            await Address.FillStates(_stateRepository);
            await Address.FillCustomers(_customerRepository);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _addressRepository.UpdateAsync(Address.ToModel());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _addressRepository.Exists(Address.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return onSuccess.Invoke();
        }
    }
}
