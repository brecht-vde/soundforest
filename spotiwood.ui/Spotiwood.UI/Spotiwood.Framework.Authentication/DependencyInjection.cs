using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Framework.Authentication.MessageHandlers;
using Spotiwood.Framework.Authentication.Options;

namespace Spotiwood.Framework.Authentication;
public static class DependencyInjection
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, WebAssemblyHostConfiguration configuration)
    {
        services.AddOptions<AuthenticationOptions>().Bind(configuration.GetRequiredSection("Authentication"));
        services.AddScoped<ApiAuthorizationMessageHandler>();

        services.AddOidcAuthentication(opt =>
        {
            configuration.Bind("Authentication", opt.ProviderOptions);
            var audience = configuration.GetRequiredSection("Authentication")["Audience"];
            opt.ProviderOptions.ResponseType = "code";
            opt.ProviderOptions.AdditionalProviderParameters.Add("audience", audience);
        });

        return services;
    }
}
