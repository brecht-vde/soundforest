﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Auth0.Authentication.Application;
using SoundForest.Clients.Auth0.Authentication.Infrastructure;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.Clients.Auth0.Authentication;
public static class DependencyInjection
{
    public static IServiceCollection AddAuth0(this IServiceCollection services, Auth0Options options)
    {
        services.AddLogging();
        services.AddSingleton<IOptions<Auth0Options>>(ctx => Options.Create<Auth0Options>(options));
        services.AddHttpClient<IAuth0Client, Auth0Client>(ctx => ctx.BaseAddress = options.BaseAddress);
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
