using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Services;

namespace EndPointEcommerce.RazorTemplates.ViewModels;

public class OrderConfirmationViewModel
{
    public required Order Order { get; set; }
    public required string OrderDetailsUrl { get; set; }
    public required string ProductImagesUrl { get; set; }

    public string AsLongDate(DateTime? value) =>
        value?.ToString("D", System.Globalization.CultureInfo.GetCultureInfo("en-US")) ?? string.Empty;

    public string AsCurrency(decimal value) =>
        value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"));

    public string? GetProductImageUrl(Image? image) =>
        ImageUrlBuilder.GetImageUrl(image, ProductImagesUrl);

    public string GetOrderUrl() => $"{OrderDetailsUrl}/{Order.OrderGuid}";
}
