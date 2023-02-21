using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Omdb.Search.Application;
using SoundForest.Clients.Omdb.Search.Infrastructure;
using SoundForest.Clients.Omdb.Search.Infrastructure.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.Clients.Omdb.Search;
public static class DependencyInjection
{
    public static IServiceCollection AddOmdb(this IServiceCollection services, OmdbOptions options)
    {
        services.AddLogging();
        services.AddSingleton(ctx => Options.Create(options));
        services.AddHttpClient<IOmdbClient, OmdbClient>(ctx => ctx.BaseAddress = options.BaseAddress);
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });

        return services;
    }
}
