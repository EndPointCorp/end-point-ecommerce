// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Taxjar;

namespace EndPointEcommerce.Infrastructure.Services;

/// <summary>
/// Service class that implements Tax Jar.
/// </summary>
public class TaxJarTaxCalculator : ITaxCalculator
{
    private const decimal SHIPPING_COST = 0.0M;
    private const string TAX_CODE = "31000";

    private readonly TaxjarApi? _taxJar;

    private readonly string? _fromCountry;
    private readonly string? _fromZip;
    private readonly string? _fromState;
    private readonly string? _fromCity;
    private readonly string? _fromStreet;

    public TaxJarTaxCalculator(IConfiguration configuration)
    {
        _taxJar = !string.IsNullOrEmpty(configuration["TaxJarApiKey"]) ?
            new TaxjarApi(configuration["TaxJarApiKey"]) : null;

        _fromCountry = configuration["TaxJarFromCountry"];
        _fromZip = configuration["TaxJarFromZip"];
        _fromState = configuration["TaxJarFromState"];
        _fromCity = configuration["TaxJarFromCity"];
        _fromStreet = configuration["TaxJarFromStreet"];
    }

    public async Task<ITaxCalculator.TaxResponse?> Calculate(Quote quote)
    {
        if (!IsConfigured) return BuildEmptyResponse();

        var parameters = BuildParameters(quote);

        var result = await _taxJar!.TaxForOrderAsync(parameters);
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

            to_country = quote.ShippingAddress!.Country!.Code,
            to_zip = quote.ShippingAddress!.ZipCode,
            to_state = quote.ShippingAddress.State?.Abbreviation,
            to_city = quote.ShippingAddress.City,
            to_street = quote.ShippingAddress.Street,

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
        new() { AmountToCollect = attributes.AmountToCollect };

    private static ITaxCalculator.TaxResponse BuildEmptyResponse() =>
        new() { AmountToCollect = 0.0M };

    private bool IsConfigured =>
        _taxJar != null  &&
        !string.IsNullOrEmpty(_fromCountry) &&
        !string.IsNullOrEmpty(_fromZip) &&
        !string.IsNullOrEmpty(_fromState) &&
        !string.IsNullOrEmpty(_fromCity) &&
        !string.IsNullOrEmpty(_fromStreet);
}
