using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointCommerce.AdminPortal.ViewModels;

namespace EndPointCommerce.AdminPortal.Pages.OrderItems
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAddressRepository _addressRepository;

        public EditModel(IOrderItemRepository orderItemRepository,
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _orderItemRepository.FindByIdWithOrderAsync(id.Value);
            if (orderItem == null)
            {
                return NotFound();
            }
            OrderItem = await OrderItemViewModel.FromModel(orderItem, _orderRepository, 
                _productRepository, _addressRepository);
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

            try
            {
                await _orderItemRepository.UpdateAsync(OrderItem.ToModel());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _orderItemRepository.Exists(OrderItem.Id))
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
