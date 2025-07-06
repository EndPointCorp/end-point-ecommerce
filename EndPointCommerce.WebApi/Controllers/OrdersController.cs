using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.WebApi.Services;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Exceptions;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderCreator _orderCreator;
        private readonly IQuoteCookieManager _quoteCookieManager;
        private readonly IQuoteResolver _quoteResolver;
        private readonly IOrderRepository _orderRepository;

        private readonly ISessionHelper _session;

        private readonly string _imagesUrlPath;

        public OrdersController(
            IOrderCreator orderCreator,
            IQuoteCookieManager quoteCookieManager,
            IQuoteResolver quoteResolver,
            IOrderRepository orderRepository,
            ISessionHelper session,
            IConfiguration config
        ) {
            _orderCreator = orderCreator;
            _quoteCookieManager = quoteCookieManager;
            _quoteResolver = quoteResolver;
            _orderRepository = orderRepository;

            _session = session;

            _imagesUrlPath = config["ProductImagesUrlPath"]!;
        }

        // GET: api/Orders
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ResourceModels.Order>>> GetOrders()
        {
            var customerId = await _session.GetCustomerId(User);

            if (customerId == null)
            {
                return NotFound();
            }

            return ResourceModels.Order.FromListOfEntities(
                await _orderRepository.FetchAllByCustomerIdAsync(customerId.Value)
            );
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceModels.Order>> GetOrder(Guid id)
        {
            var order = await _orderRepository.FindByGuidWithItemsAsync(id);

            if (order == null) return NotFound();

            return ResourceModels.Order.FromEntity(order);
        }

        // POST: api/Orders/
        [HttpPost]
        public async Task<ActionResult<ResourceModels.Order>> PostOrder(
            [FromBody] ResourceModels.OrderPost order
        ) {
            var quote = await _quoteResolver.ResolveQuote(User, Request);
            if (quote == null) return NotFound();

            try
            {
                var result = await _orderCreator.Run(new() {
                    QuoteId = quote.Id,
                    PaymentMethodNonceValue = order.PaymentMethodNonceValue,
                    PaymentMethodNonceDescriptor = order.PaymentMethodNonceDescriptor
                });

                _quoteCookieManager.DeleteQuoteIdCookie(Response);

                return ResourceModels.Order.FromEntity(result, _imagesUrlPath);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (DomainValidationException ex)
            {
                return BadRequest(ex.ToDictionary());
            }
        }
    }
}
