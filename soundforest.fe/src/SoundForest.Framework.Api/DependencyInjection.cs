using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SoundForest.Framework.Api.Application.Abstractions;
using SoundForest.Framework.Api.Infrastructure;
using SoundForest.Framework.Authentication;
using SoundForest.Framework.Authentication.MessageHandlers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.Framework.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, WebAssemblyHostConfiguration configuration)
    {
        services.AddAuthentication(configuration);

        services.AddHttpClient<IService, Service>(client
            // TODO: Get from config
            => client.BaseAddress = new Uri("http://localhost:7080"))
            .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

        // TODO: replace with IOptions
        services.AddSingleton<JsonSerializerOptions>(ctx =>
        {
            var defaultOptions = new JsonSerializerOptions();
            defaultOptions.Converters.Add(new JsonStringEnumConverter());
            defaultOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            defaultOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            defaultOptions.PropertyNameCaseInsensitive = true;
            return defaultOptions;
        });

        return services;
    }
}
