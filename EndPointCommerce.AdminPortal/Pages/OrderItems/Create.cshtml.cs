using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.OrderItems
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAddressRepository _addressRepository;

        public CreateModel(IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IAddressRepository addressRepository)
        {
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _addressRepository = addressRepository;
        }

        [BindProperty]
        public OrderItemViewModel OrderItem { get; set; } = default!;


        public async Task<IActionResult> OnGet(int orderId)
        {
            OrderItem = await OrderItemViewModel.CreateDefault(orderId, _orderRepository, _productRepository, _addressRepository);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return await HandlePost(() => RedirectToPage("/Orders/Edit", new { id = OrderItem.OrderId }));
        }

        public async Task<IActionResult> OnPostSaveAndContinueAsync()
        {
            return await HandlePost(() => RedirectToPage("./Edit", new { OrderItem.Id }));
        }

        private async Task<IActionResult> HandlePost(Func<IActionResult> onSuccess)
        {
            await OrderItem.FillProducts(_productRepository);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var orderItem = OrderItem.ToModel();

            await _orderItemRepository.AddAsync(orderItem);

            OrderItem.Id = orderItem.Id;

            return onSuccess.Invoke();
        }
    }
}
