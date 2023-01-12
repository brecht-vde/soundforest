﻿using System.Text.Json;

namespace SoundForest.Clients.Auth0.Authentication.Application.Responses.Extensions;
internal static class ResponseExtensions
{
    public async static Task<T?> ToResponse<T>(this Stream source, JsonSerializerOptions serializer)
    {
        serializer ??= JsonSerializerOptions.Default;
        var response = await JsonSerializer.DeserializeAsync<T>(source, serializer);
        return response;
    }
}
