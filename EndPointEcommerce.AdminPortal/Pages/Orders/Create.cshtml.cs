using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.AdminPortal.ViewModels;

namespace EndPointEcommerce.AdminPortal.Pages.Orders
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IAddressRepository _addressRepository;

        public CreateModel(IOrderRepository orderRepository,
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


        public async Task<IActionResult> OnGet()
        {
            Order = await OrderViewModel.CreateDefault(0, 0, 0, 0, 0, _orderStatusRepository, _customerRepository,
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
                return Page();
            }

            var order = Order.ToModel();

            await _orderRepository.AddAsync(order);

            Order.Id = order.Id;

            return onSuccess.Invoke();
        }
    }
}
