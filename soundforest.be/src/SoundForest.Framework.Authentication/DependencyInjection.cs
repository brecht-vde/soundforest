﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SoundForest.Framework.Authentication.Application.Abstractions;
using SoundForest.Framework.Authentication.Application.Middleware;
using SoundForest.Framework.Authentication.Application.Sentinels;
using SoundForest.Framework.Authentication.Infrastructure.Authenticators;
using SoundForest.Framework.Authentication.Infrastructure.Options;
using System.Reflection;

namespace SoundForest.Framework.Authentication;
public static class DependencyInjection
{
    public static IServiceCollection AddFunctionAuthentication(this IServiceCollection services, string? audience, Uri? issuer, params Assembly[] assemblies)
    {
        if (string.IsNullOrWhiteSpace(audience))
            throw new ArgumentNullException(nameof(audience));

        if (issuer is null)
            throw new ArgumentNullException(nameof(issuer));

        if (assemblies is null || assemblies.Length is 0)
            throw new ArgumentNullException(nameof(assemblies));

        services.AddSingleton(context =>
        {
            var settings = new AuthenticationOptions() { Issuer = issuer, Audience = audience };
            return Options.Create(settings);
        });

        services.AddSingleton<IAuthenticator, JwtAuthenticator>();
        services.TryAddSingleton<IConfigurationManager<OpenIdConnectConfiguration>>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<AuthenticationOptions>>();
            var retriever = new HttpDocumentRetriever() { RequireHttps = true };
            var manager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{settings.Value.Issuer}.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                retriever);

            return manager;
        });

        services.TryAddSingleton<IFunctionSentinel>(context =>
        {
            return new FunctionSentinel(assemblies);
        });

        return services;
    }

    public static IFunctionsWorkerApplicationBuilder UseFunctionAuthentication(this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.UseWhen<FunctionAuthenticationMiddleware>(context =>
        {
            var sentinel = context.InstanceServices.GetRequiredService<IFunctionSentinel>();
            return sentinel.RequiresElevation(context.FunctionDefinition.Name);
        });

        return builder;
    }
}
