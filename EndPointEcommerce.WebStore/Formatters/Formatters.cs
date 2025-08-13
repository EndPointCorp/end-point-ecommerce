using System.Globalization;

namespace EndPointEcommerce.WebStore.Formatters;

public static class Formatters
{
    public static string AsLongDate(this DateTime value) =>
        value.ToString("D", CultureInfo.GetCultureInfo("en-US"));

    public static string AsCurrency(this decimal value) =>
        value.ToString("C", CultureInfo.GetCultureInfo("en-US"));
}
