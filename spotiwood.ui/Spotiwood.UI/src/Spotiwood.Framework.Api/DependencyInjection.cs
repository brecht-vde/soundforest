using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Framework.Api.Application.Abstractions;
using Spotiwood.Framework.Api.Infrastructure;
using Spotiwood.Framework.Authentication;
using Spotiwood.Framework.Authentication.MessageHandlers;

namespace Spotiwood.Framework.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, WebAssemblyHostConfiguration configuration)
    {
        services.AddAuthentication(configuration);

        services.AddHttpClient<IService, Service>(client
            => client.BaseAddress = new Uri("http://localhost:7070"))
            .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

        return services;
    }
}
