using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Spotify.Authentication.Application;
using SoundForest.Clients.Spotify.Authentication.Domain;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Mappings;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Options;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Responses;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Responses.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SoundForest.Clients.Spotify.Authentication.Infrastructure;
internal sealed class SpotifyAuthClient : ISpotifyAuthClient
{
    private readonly ILogger<SpotifyAuthClient> _logger;
    private readonly IOptions<SpotifyAuthOptions> _options;
    private readonly HttpClient _client;
    private readonly IOptions<JsonSerializerOptions> _serializer;

    private static Func<string?, string?, string?> EncodeCredentials = (clientId, clientSecret)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

    private static Func<FormUrlEncodedContent> AccessContent = ()
        => new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

    private static Func<string?, string?, FormUrlEncodedContent> RefreshContent = (clientId, token)
        => new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("refresh_token", token)
            });

    public SpotifyAuthClient(ILogger<SpotifyAuthClient> logger, IOptions<SpotifyAuthOptions> options, HttpClient client, IOptions<JsonSerializerOptions> serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        if (string.IsNullOrWhiteSpace(_options?.Value?.ClientId))
            throw new ArgumentNullException(nameof(SpotifyAuthOptions.ClientId));

        if (string.IsNullOrWhiteSpace(_options?.Value?.ClientSecret))
            throw new ArgumentNullException(nameof(SpotifyAuthOptions.ClientSecret));

        if (_options?.Value?.BaseAddress is null)
            throw new ArgumentNullException(nameof(SpotifyAuthOptions.BaseAddress));
    }

    public async Task<Token?> AccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await TokenAsync(AccessContent(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not request accesstoken.");
            return null;
        }
    }

    public async Task<Token?> RefreshTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        try
        {
            return await TokenAsync(RefreshContent(_options.Value.ClientId, token), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not request refresh token.");
            return null;
        }
    }

    private async Task<Token?> TokenAsync(FormUrlEncodedContent content, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"/api/token", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeCredentials(_options.Value.ClientId, _options.Value.ClientSecret));
        request.Content = content;

        var response = await _client.SendAsync(request);
        using var stream = await response.Content.ReadAsStreamAsync();
        var tokenResponse = await stream.ToResponse<TokenResponse>(_serializer.Value);

        return tokenResponse?.ToToken();
    }
}
