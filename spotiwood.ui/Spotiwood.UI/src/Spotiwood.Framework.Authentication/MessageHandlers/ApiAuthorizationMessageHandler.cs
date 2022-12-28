using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Spotiwood.Framework.Authentication.Options;

namespace Spotiwood.Framework.Authentication.MessageHandlers;
public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    private readonly IOptions<AuthenticationOptions>? _options;

    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager,
        IOptions<AuthenticationOptions> options)
        : base(provider, navigationManager)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        ConfigureHandler(authorizedUrls: new[] { _options?.Value?.ApiRoot?.AbsoluteUri });
    }
}
