using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.WebApi.ResourceModels;
using EndPointCommerce.WebApi.Services;
using EndPointCommerce.Domain.Services;
using EndPointCommerce.Domain.Exceptions;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteUpdater _quoteUpdater;
        private readonly IQuoteItemCreator _quoteItemCreator;
        private readonly IQuoteItemUpdater _quoteItemUpdater;
        private readonly IQuoteItemDeleter _quoteItemDeleter;
        private readonly IQuoteValidator _quoteValidator;

        private readonly IQuoteCookieManager _quoteCookieManager;
        private readonly IQuoteResolver _quoteResolver;
        private readonly ISessionHelper _session;

        private readonly string _imagesUrl;

        public QuoteController(
            IQuoteUpdater quoteUpdater,
            IQuoteItemCreator quoteItemCreator,
            IQuoteItemUpdater quoteItemUpdater,
            IQuoteItemDeleter quoteItemDeleter,
            IQuoteValidator quoteValidator,
            IQuoteCookieManager quoteCookieManager,
            IQuoteResolver quoteResolver,
            ISessionHelper session,
            IConfiguration config
        ) {
            _quoteUpdater = quoteUpdater;
            _quoteItemCreator = quoteItemCreator;
            _quoteItemUpdater = quoteItemUpdater;
            _quoteItemDeleter = quoteItemDeleter;
            _quoteValidator = quoteValidator;

            _quoteCookieManager = quoteCookieManager;
            _quoteResolver = quoteResolver;
            _session = session;

            _imagesUrl = config["ProductImagesUrl"]!;
        }

        // GET: api/Quote
        [HttpGet]
        public async Task<ActionResult<Quote>> GetQuote()
        {
            var quote = await _quoteResolver.ResolveQuote(User, Request);
            if (quote == null) return NotFound();

            _quoteCookieManager.SetQuoteIdCookie(Response, quote.Id);

            return Quote.FromEntity(quote, _imagesUrl);
        }

        // PUT: api/Quote
        [HttpPut]
        public async Task<ActionResult<Quote>> PutQuote(
            [FromBody] QuotePut payload
        ) {
            try
            {
                var quote = await _quoteResolver.ResolveQuote(User, Request);
                var customerId = await _session.GetCustomerId(User);

                var result = await _quoteUpdater.Run(new() {
                    QuoteId = quote?.Id,
                    CustomerId = customerId,
                    Email = payload.Email,
                    ShippingAddressId = payload.ShippingAddressId,
                    ShippingAddress = payload.ShippingAddress?.ToEntity(),
                    BillingAddressId = payload.BillingAddressId,
                    BillingAddress = payload.BillingAddress?.ToEntity(),
                    CouponCode = payload.CouponCode,
                });

                _quoteCookieManager.SetQuoteIdCookie(Response, result.Id);

                return Quote.FromEntity(result, _imagesUrl);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/Quote/Items
        [HttpPost("Items")]
        public async Task<ActionResult<QuoteItem>> PostQuoteItem(
            [FromBody] QuoteItemPost quoteItem
        ) {
            try
            {
                var quote = await _quoteResolver.ResolveQuote(User, Request);
                var customerId = await _session.GetCustomerId(User);

                var result = await _quoteItemCreator.Run(new() {
                    QuoteId = quote?.Id,
                    CustomerId = customerId,
                    ProductId = quoteItem.ProductId,
                    Quantity = quoteItem.Quantity
                });

                _quoteCookieManager.SetQuoteIdCookie(Response, result.QuoteId);

                return QuoteItem.FromEntity(result, _imagesUrl);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        // PUT: api/Quote/Items/{id}
        [HttpPut("Items/{id}")]
        public async Task<ActionResult<QuoteItem>> PutQuoteItem(
            int id,
            [FromBody] QuoteItemPut quoteItem
        ) {
            return await HandleUpdateQuoteItem(
                async quoteId => await _quoteItemUpdater.Run(new() {
                    QuoteItemId = id,
                    QuoteId = quoteId,
                    Quantity = quoteItem.Quantity
                }),
                result => QuoteItem.FromEntity(result, _imagesUrl)
            );
        }

        // DELETE: api/Quote/Items/{id}
        [HttpDelete("Items/{id}")]
        public async Task<ActionResult> DeleteQuoteItem(int id)
        {
            return await HandleUpdateQuoteItem<object?>(
                async quoteId => {
                    await _quoteItemDeleter.Run(new() {
                        QuoteId = quoteId,
                        QuoteItemId = id
                    });

                    return null;
                }
            );
        }

        // POST: api/Quote/Validate
        [HttpPost("Validate")]
        public async Task<ActionResult<Quote>> PostQuoteValidate()
        {
            var quote = await _quoteResolver.ResolveQuote(User, Request);
            if (quote == null) return NotFound();

            try
            {
                var result = await _quoteValidator.Run(quote.Id);

                return Quote.FromEntity(result, _imagesUrl);
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

        private async Task<ActionResult> HandleUpdateQuoteItem<T>(
            Func<int, Task<T?>> updater,
            Func<T, object?>? responseBuilder = null
        ) {
            var quote = await _quoteResolver.ResolveQuote(User, Request);
            if (quote == null) return NotFound();

            try
            {
                var result = await updater.Invoke(quote.Id);

                _quoteCookieManager.SetQuoteIdCookie(Response, quote.Id);

                if (result != null)
                    return Ok(responseBuilder!.Invoke(result));
                else
                    return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
