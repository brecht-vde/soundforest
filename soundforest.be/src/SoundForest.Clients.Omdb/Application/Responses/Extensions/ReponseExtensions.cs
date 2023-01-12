using System.Text.Json;

namespace SoundForest.Clients.Omdb.Application.Responses.Extensions;
internal static class ReponseExtensions
{
    public static bool IsErrorResponse(this string content, JsonSerializerOptions? serializer)
    {
        serializer ??= JsonSerializerOptions.Default;
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, serializer);
        return errorResponse?.Response?.Equals("false", StringComparison.OrdinalIgnoreCase) is true;
    }

    public static T? ToResponse<T>(this string content, JsonSerializerOptions? serializer)
    {
        serializer ??= JsonSerializerOptions.Default;
        var response = JsonSerializer.Deserialize<T>(content, serializer);
        return response;
    }
}
