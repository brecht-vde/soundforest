using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Spotify.Authentication.Application;
using SoundForest.Clients.Spotify.Authentication.Application.Options;
using SoundForest.Clients.Spotify.Authentication.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.Clients.Spotify.Authentication;
public static class DependencyInjection
{
    public static IServiceCollection AddSpotifyAuth(this IServiceCollection services, SpotifyAuthOptions options)
    {
        services.AddLogging();
        services.AddSingleton<IOptions<SpotifyAuthOptions>>(ctx => Options.Create<SpotifyAuthOptions>(options));
        services.AddHttpClient<ISpotifyAuthClient, SpotifyAuthClient>(ctx => ctx.BaseAddress = options.BaseAddress);
        services.AddSingleton<JsonSerializerOptions>(ctx =>
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            return options;
        });

        return services;
    }
}
