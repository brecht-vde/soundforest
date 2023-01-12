using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Omdb.Application;
using SoundForest.Clients.Omdb.Application.Options;
using SoundForest.Clients.Omdb.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.Clients.Omdb;
public static class DependencyInjection
{
    public static IServiceCollection AddOmdb(this IServiceCollection services, OmdbOptions options)
    {
        services.AddLogging();
        services.AddSingleton<IOptions<OmdbOptions>>(ctx => Options.Create<OmdbOptions>(options));
        services.AddHttpClient<IOmdbClient, OmdbClient>(ctx => ctx.BaseAddress = options.BaseAddress);
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
