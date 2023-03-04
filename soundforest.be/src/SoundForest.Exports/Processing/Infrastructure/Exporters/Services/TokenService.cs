using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SoundForest.Clients.Auth0.Authentication.Application;
using SoundForest.Clients.Spotify.Authentication.Application;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
internal sealed class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly ISpotifyAuthClient _spotifyAuthClient;
    private readonly IAuth0Client _auth0Client;
    private readonly IMemoryCache _cache;

    public TokenService(
        ILogger<TokenService> logger,
        ISpotifyAuthClient spotifyAuthClient,
        IAuth0Client auth0Client,
        IMemoryCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _spotifyAuthClient = spotifyAuthClient ?? throw new ArgumentNullException(nameof(spotifyAuthClient));
        _auth0Client = auth0Client ?? throw new ArgumentNullException(nameof(auth0Client));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<string?> GetM2mAccesToken(CancellationToken cancellationToken = default)
    {
        try
        {
            var m2mToken = await _cache.GetOrCreateAsync(
                key: $"spotify:auth:m2m",
                factory: async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);

                    var spotifyM2mToken = await _spotifyAuthClient.AccessTokenAsync(cancellationToken);

                    if (string.IsNullOrWhiteSpace(spotifyM2mToken?.AccessToken)) return null;

                    entry.AbsoluteExpirationRelativeToNow = spotifyM2mToken?.ExpiresIn is not null
                        ? TimeSpan.FromSeconds(spotifyM2mToken.ExpiresIn.Value - 300)
                        : TimeSpan.FromSeconds(1);

                    return spotifyM2mToken?.AccessToken;
                });

            return m2mToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not acquire m2m access token.");
            return null;
        }
    }

    public async Task<string?> GetUserAccessToken(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var userToken = await _cache.GetOrCreateAsync(
                key: $"spotify:auth:{username}",
                factory: async entry =>
                {
                    try
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);

                        var auth0M2mToken = await _auth0Client.M2mTokenAsync(cancellationToken);

                        if (string.IsNullOrWhiteSpace(auth0M2mToken?.AccessToken)) return null;

                        var auth0UserToken = await _auth0Client.UserTokenAsync(auth0M2mToken.AccessToken, username, cancellationToken);

                        if (string.IsNullOrWhiteSpace(auth0UserToken?.RefreshToken)) return null;

                        var spotifyUserToken = await _spotifyAuthClient.RefreshTokenAsync(auth0UserToken?.RefreshToken, cancellationToken);

                        if (string.IsNullOrWhiteSpace(spotifyUserToken?.AccessToken)) return null;

                        entry.AbsoluteExpirationRelativeToNow = spotifyUserToken?.ExpiresIn is not null
                            ? TimeSpan.FromSeconds(spotifyUserToken.ExpiresIn.Value - 300)
                            : TimeSpan.FromSeconds(1);

                        return spotifyUserToken?.AccessToken;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Could not build user access token.");
                        return null;
                    }
                });

            return userToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not acquire user access token.");
            return null;
        }
    }
}
