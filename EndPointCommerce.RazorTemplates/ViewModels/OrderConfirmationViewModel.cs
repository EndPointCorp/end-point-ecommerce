using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Services;

namespace EndPointCommerce.RazorTemplates.ViewModels;

public class OrderConfirmationViewModel
{
    public required Order Order { get; set; }
    public required string OrderDetailsUrlPath { get; set; }
    public required string ProductImagesUrlPath { get; set; }

    public string AsLongDate(DateTime? value) =>
        value?.ToString("D", System.Globalization.CultureInfo.GetCultureInfo("en-US")) ?? string.Empty;

    public string AsCurrency(decimal value) =>
        value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"));

    public string? GetProductImageUrl(Image? image) =>
        ImageUrlBuilder.GetImageUrl(image, ProductImagesUrlPath);

    public string GetOrderUrl() => $"{OrderDetailsUrlPath}/{Order.OrderGuid}";
}
