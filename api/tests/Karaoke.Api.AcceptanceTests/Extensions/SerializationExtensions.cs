using System.Text.Json;

namespace Karaoke.Api.AcceptanceTests.Extensions;

public static class SerializationExtensions
{
    public static async Task<T> ParseAs<T>(this HttpResponseMessage response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, options);
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }
}