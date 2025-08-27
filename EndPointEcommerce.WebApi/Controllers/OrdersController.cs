// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using EndPointEcommerce.WebApi.Services;
using EndPointEcommerce.Domain.Services;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EndPointEcommerce.WebApi.ResourceModels;

namespace EndPointEcommerce.WebApi.Controllers
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

        private readonly string _imagesUrl;

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

            _imagesUrl = config["ProductImagesUrl"]!;
        }

        // GET: api/Orders
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var customerId = await _session.GetCustomerId(User);

            if (customerId == null) return NotFound(new ErrorMessage("Customer not found"));

            return Order.FromListOfEntities(
                await _orderRepository.FetchAllByCustomerIdAsync(customerId.Value)
            );
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _orderRepository.FindByGuidWithItemsAsync(id);

            if (order == null) return NotFound(new ErrorMessage("Order not found"));

            return Order.FromEntity(order);
        }

        // POST: api/Orders/
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(
            [FromBody] OrderPost order
        ) {
            var quote = await _quoteResolver.ResolveQuote(User, Request);
            if (quote == null) return NotFound(new ErrorMessage("Quote not found"));

            try
            {
                var result = await _orderCreator.Run(new() {
                    QuoteId = quote.Id,
                    PaymentMethodNonceValue = order.PaymentMethodNonceValue,
                    PaymentMethodNonceDescriptor = order.PaymentMethodNonceDescriptor
                });

                _quoteCookieManager.DeleteQuoteIdCookie(Response);

                return Order.FromEntity(result, _imagesUrl);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorMessage(ex));
            }
            catch (DomainValidationException ex)
            {
                return BadRequest(ex.ToDictionary());
            }
        }
    }
}
