using System.Text.Json;

namespace EndPointCommerce.WebStoreSpa.Api;

public static class HttpResponseMessageExtensions
{
    public static async Task<List<string>> GetErrorMessages(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var errors = json.RootElement.GetProperty("errors");

            return errors
                .EnumerateObject()
                .Where(prop => prop.Name != "DuplicateUserName")
                .Select(prop => string.Join(" ", prop.Value.EnumerateArray().Select(v => v.GetString())))
                .ToList();
        }

        return [];
    }
}
