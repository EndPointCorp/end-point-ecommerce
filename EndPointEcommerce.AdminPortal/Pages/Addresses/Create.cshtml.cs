// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Addresses
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICustomerRepository _customerRepository;

        public CreateModel(
            IAddressRepository addressRepository,
            ICountryRepository countryRepository,
            IStateRepository stateRepository,
            ICustomerRepository customerRepository
        ) {
            _addressRepository = addressRepository;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public AddressViewModel Address { get; set; } = default!;

        public async Task<IActionResult> OnGet(int customerId)
        {
            Address = await AddressViewModel.CreateDefault(customerId, _countryRepository, _stateRepository, _customerRepository);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index", new { customerId = Address.CustomerId }));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { Address.Id }));
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            await Address.FillCountries(_countryRepository);
            await Address.FillStates(_stateRepository);
            await Address.FillCustomers(_customerRepository);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var address = Address.ToModel();

            await _addressRepository.AddAsync(address);

            Address.Id = address.Id;

            return onSuccess.Invoke();
        }
    }
}
