using System.ComponentModel.DataAnnotations;
using EndPointCommerce.WebStore.Api;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.WebStore.Pages.Checkout;

public class ShippingModel : BasePageModel
{
    public ShippingModel(IApiClient apiClient) : base(apiClient) { }

    private const int US_COUNTRY_ID = 225;

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [BindProperty]
    [Required]
    public Address ShippingAddress { get; set; } = default!;

    [BindProperty]
    [Required]
    public Address BillingAddress { get; set; } = default!;

    [BindProperty]
    public bool UseShippingAsBilling { get; set; } = true;

    public async Task<IActionResult> OnGetAsync()
    {
        await FetchCategories();
        await FetchStates();
        await FetchQuote();

        if (QuoteIsEmpty) return RedirectToPage("/Cart");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await FetchQuote();
        if (QuoteIsEmpty) return RedirectToPage("/Cart");

        ShippingAddress.CountryId = US_COUNTRY_ID;
        BillingAddress.CountryId = US_COUNTRY_ID;

        ResolveBillingAddress();

        if (!ModelState.IsValid)
        {
            await FetchCategories();
            await FetchStates();
            return Page();
        }

        var response = await _apiClient.PutQuote(Email, ShippingAddress, BillingAddress, QuoteCookie);
        if (response.Cookie != null) QuoteCookie = response.Cookie;

        return RedirectToPage("/Checkout/Payment");
    }

    private void ResolveBillingAddress()
    {
        if (UseShippingAsBilling)
        {
            BillingAddress = new Address
            {
                Name = ShippingAddress.Name,
                LastName = ShippingAddress.LastName,
                Street = ShippingAddress.Street,
                StreetTwo = ShippingAddress.StreetTwo,
                City = ShippingAddress.City,
                CountryId = ShippingAddress.CountryId,
                StateId = ShippingAddress.StateId,
                ZipCode = ShippingAddress.ZipCode,
                PhoneNumber = ShippingAddress.PhoneNumber
            };

            // Remove all ModelState errors for BillingAddress properties
            foreach (var item in ModelState.FindKeysWithPrefix("BillingAddress"))
            {
                ModelState.Remove(item.Key);
            }
        }
    }
}

