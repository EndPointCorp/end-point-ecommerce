using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Taxjar;

namespace EndPointCommerce.Infrastructure.Services;

/// <summary>
/// Service class that implements Tax Jar.
/// </summary>
public class TaxJarTaxCalculator: ITaxCalculator
{
    private const string TAX_COUNTRY_CODE = "US";
    private const decimal SHIPPING_COST = 0.0M;
    private const string TAX_CODE = "31000";

    private readonly TaxjarApi _taxJar;

    private readonly string _fromCountry;
    private readonly string _fromZip;
    private readonly string _fromState;
    private readonly string _fromCity;
    private readonly string _fromStreet;

    public TaxJarTaxCalculator(IConfiguration configuration)
    {
        _taxJar = new TaxjarApi(
            configuration["TaxJarApiKey"] ??
                throw new InvalidOperationException("Config setting 'TaxJarApiKey' not found.")
        );

        _fromCountry = configuration["TaxJarFromCountry"] ??
            throw new InvalidOperationException("Config setting 'TaxJarFromCountry' not found.");
        _fromZip = configuration["TaxJarFromZip"] ??
            throw new InvalidOperationException("Config setting 'TaxJarFromZip' not found.");
        _fromState = configuration["TaxJarFromState"] ??
            throw new InvalidOperationException("Config setting 'TaxJarFromState' not found.");
        _fromCity = configuration["TaxJarFromCity"] ??
            throw new InvalidOperationException("Config setting 'TaxJarFromCity' not found.");
        _fromStreet = configuration["TaxJarFromStreet"] ??
            throw new InvalidOperationException("Config setting 'TaxJarFromStreet' not found.");
    }

    public async Task<ITaxCalculator.TaxResponse?> Calculate(Quote quote)
    {
        var parameters = BuildParameters(quote);

        var result = await _taxJar.TaxForOrderAsync(parameters);
        if (result == null) return null;

        var response = BuildResponse(result);

        return response;
    }

    private object BuildParameters(Quote quote) =>
        new
        {
            from_country = _fromCountry,
            from_zip = _fromZip,
            from_state = _fromState,
            from_city = _fromCity,
            from_street = _fromStreet,

            to_country = TAX_COUNTRY_CODE,
            to_zip = quote.ShippingAddress!.ZipCode,
            to_state = quote.ShippingAddress.State.Abbreviation,
            to_city = quote.ShippingAddress.City,
            tp_street = quote.ShippingAddress.Street,

            shipping = SHIPPING_COST,

            line_items = quote.Items.Select(item => new
            {
                id = item.Id,
                unit_price = item.UnitPrice,
                quantity = item.Quantity,
                product_tax_code = TAX_CODE,
                discount = item.Discount
            }).ToList()
        };

    private static ITaxCalculator.TaxResponse BuildResponse(TaxResponseAttributes attributes) =>
        new()
        {
            AmountToCollect = attributes.AmountToCollect,
        };
}
