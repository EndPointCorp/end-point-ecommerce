using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Exceptions;
using EndPointEcommerce.Domain.Interfaces;
using static EndPointEcommerce.Domain.Services.IQuoteUpdater;

namespace EndPointEcommerce.Domain.Services;

public interface IQuoteUpdater
{
    public class InputPayload
    {
        public int? QuoteId { get; set; }
        public int? CustomerId { get; set; }
        public string? Email { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }
        public int? BillingAddressId { get; set; }
        public Address? BillingAddress { get; set; }
        public string? CouponCode { get; set; }

        public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
        public bool HasCouponCode => !string.IsNullOrWhiteSpace(CouponCode);
        public bool HasShippingAddress => ShippingAddressId.HasValue || ShippingAddress != null;
        public bool HasBillingAddress => BillingAddressId.HasValue || BillingAddress != null;
    }

    Task<Quote> Run(InputPayload payload);
}

public class QuoteUpdater : BaseQuoteService, IQuoteUpdater
{
    private readonly ICouponRepository _couponRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IStateRepository _stateRepository;

    public QuoteUpdater(
        IQuoteRepository quoteRepository,
        IQuoteItemRepository quoteItemRepository,
        IQuoteTaxCalculator quoteTaxCalculator,
        ICouponRepository couponRepository,
        IAddressRepository addressRepository,
        ICountryRepository countryRepository,
        IStateRepository stateRepository
    ) : base(quoteRepository, quoteItemRepository, quoteTaxCalculator)
    {
        _couponRepository = couponRepository;
        _addressRepository = addressRepository;
        _countryRepository = countryRepository;
        _stateRepository = stateRepository;
    }

    public async Task<Quote> Run(InputPayload payload)
    {
        var quote = await FindOrCreateQuoteOrThrow(payload.QuoteId, payload.CustomerId);

        await ApplyUpdate(quote, payload);
        await _quoteRepository.UpdateAsync(quote);

        return quote;
    }

    private async Task ApplyUpdate(Quote quote, InputPayload payload)
    {
        var shouldUpdateTax = ShouldUpdateTax(quote, payload);

        if (payload.HasEmail) quote.Email = payload.Email;

        if (payload.HasShippingAddress)
        {
            quote.ShippingAddress = await ResolveAddressOrThrow(
                payload.ShippingAddress, payload.ShippingAddressId, quote
            );
        }

        if (payload.HasBillingAddress)
        {
            quote.BillingAddress = await ResolveAddressOrThrow(
                payload.BillingAddress, payload.BillingAddressId, quote
            );
        }

        if (payload.HasCouponCode)
        {
            var coupon = await _couponRepository.FindByCodeAsync(payload.CouponCode!) ??
                throw new EntityNotFoundException("Coupon not found");

            quote.Coupon = coupon;
        }
        else
        {
            quote.Coupon = null;
        }

        if (shouldUpdateTax) await UpdateTax(quote);
    }

    private async Task<Address> ResolveAddressOrThrow(Address? address, int? addressId, Quote quote)
    {
        var resolvedAddress = address ??
            (await _addressRepository.FindByIdAsync(addressId!.Value))?.Clone() ??
                throw new EntityNotFoundException("Address not found");

        resolvedAddress.Country ??= await _countryRepository.FindByIdAsync(resolvedAddress.CountryId) ??
                throw new EntityNotFoundException("Country not found");

        if (resolvedAddress.StateId != null)
            resolvedAddress.State ??= await _stateRepository.FindByIdAsync(resolvedAddress.StateId.Value) ??
                    throw new EntityNotFoundException("State not found");

        if (quote.IsFromCustomer)
            resolvedAddress.CustomerId = quote.CustomerId;

        return resolvedAddress;
    }

    private static bool ShouldUpdateTax(Quote quote, InputPayload payload)
    {
        if (!payload.HasShippingAddress && !payload.HasCouponCode) return false;

        var isCouponUpdated = payload.HasCouponCode &&
            (quote.Coupon == null || !quote.Coupon.CodeIs(payload.CouponCode!));

        var isShippingAddressUpdated = payload.HasShippingAddress &&
            (quote.ShippingAddress == null || !quote.ShippingAddress.Equals(payload.ShippingAddress!));

        return isCouponUpdated || isShippingAddressUpdated;
    }
}
