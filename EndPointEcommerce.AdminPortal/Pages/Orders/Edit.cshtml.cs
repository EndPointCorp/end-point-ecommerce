using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Orders
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IAddressRepository _addressRepository;

        public EditModel(IOrderRepository orderRepository,
            IOrderStatusRepository orderStatusRepository, ICustomerRepository customerRepository,
            IPaymentMethodRepository paymentMethodRepository, IAddressRepository addressRepository)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _addressRepository = addressRepository;
        }

        [BindProperty]
        public OrderViewModel Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.FindByIdWithItemsAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }
            Order = await OrderViewModel.FromModel(order, _orderStatusRepository, _customerRepository,
                _paymentMethodRepository);
            return Page();
        }

        public async Task<JsonResult> OnGetCustomerBillingAddresses(int customerId)
        {
            var addresses = await _addressRepository.FetchAllByCustomerIdAsync(customerId);
            return new JsonResult(addresses);
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("./Index"));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { Order.Id }));
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFormOptions();
                return Page();
            }

            try
            {
                await _orderRepository.UpdateAsync(Order.ToModel());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _orderRepository.Exists(Order.Id))
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

        protected async Task PopulateFormOptions()
        {
            var order = await _orderRepository.FindByIdWithItemsAsync(Order.Id);
            if (order != null)
                Order = await OrderViewModel.FromModel(order, _orderStatusRepository, _customerRepository,
                    _paymentMethodRepository);
        }
    }
}
