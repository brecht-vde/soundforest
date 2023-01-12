using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SoundForest.Framework.Authentication.Application.Abstractions;
using SoundForest.Framework.Authentication.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;

namespace SoundForest.Framework.Authentication.Infrastructure.Authenticators;
internal sealed class JwtAuthenticator : IAuthenticator
{
    private readonly IOptions<AuthenticationOptions> _options;
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _manager;

    public JwtAuthenticator(IOptions<AuthenticationOptions> options, IConfigurationManager<OpenIdConnectConfiguration> manager)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    public async Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        token = token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

        var config = await _manager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);

        var parameters = new TokenValidationParameters()
        {
            ValidAudience = _options.Value.Audience,
            ValidIssuer = _options.Value.Issuer!.AbsoluteUri,
            IssuerSigningKeys = config.SigningKeys,
            ValidateIssuerSigningKey = true
        };

        var handler = new JwtSecurityTokenHandler();

        var result = await handler.ValidateTokenAsync(token, parameters).ConfigureAwait(false);

        return result.IsValid;
    }
}
