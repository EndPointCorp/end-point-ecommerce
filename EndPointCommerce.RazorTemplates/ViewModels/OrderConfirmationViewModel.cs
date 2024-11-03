using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.RazorTemplates.ViewModels;

public class OrderConfirmationViewModel
{
    public required Order Order { get; set; }
    public required string WebsiteShippingInfoUrl { get; set; }

    public string AsLongDate(DateTime? value) =>
        value?.ToString("D", System.Globalization.CultureInfo.GetCultureInfo("en-US")) ?? string.Empty;

    public string AsCurrency(decimal value) =>
        value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
}
