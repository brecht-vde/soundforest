using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Auth0.Authentication.Application;
using SoundForest.Clients.Auth0.Authentication.Domain;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Mappings;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Options;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Responses;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Responses.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SoundForest.Clients.Auth0.Authentication.Infrastructure;
internal sealed class Auth0Client : IAuth0Client
{
    private ILogger<Auth0Client> _logger;
    private readonly IOptions<Auth0Options> _options;
    private readonly HttpClient _client;
    private readonly IOptions<JsonSerializerOptions> _serializer;

    public Auth0Client(
        ILogger<Auth0Client> logger,
        IOptions<Auth0Options> options,
        HttpClient client,
        IOptions<JsonSerializerOptions> serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        if (string.IsNullOrWhiteSpace(_options?.Value?.ClientId))
            throw new ArgumentNullException(nameof(Auth0Options.ClientId));

        if (string.IsNullOrWhiteSpace(_options?.Value?.ClientSecret))
            throw new ArgumentNullException(nameof(Auth0Options.ClientSecret));

        if (_options?.Value?.BaseAddress is null)
            throw new ArgumentNullException(nameof(Auth0Options.BaseAddress));

        if (_options?.Value?.BaseAddress is null)
            throw new ArgumentNullException(nameof(Auth0Options.BaseAddress));
    }

    public async Task<Token?> M2mTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new
            {
                client_id = _options.Value.ClientId,
                client_secret = _options.Value.ClientSecret,
                audience = _options.Value.Audience,
                grant_type = "client_credentials"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/oauth/token", UriKind.Relative));
            request.Content = JsonContent.Create(body);

            var response = await _client.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();
            var tokenResponse = await stream.ToResponse<TokenResponse>(_serializer.Value);

            return tokenResponse?.ToToken();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not request auth0 access token");
            return null;
        }
    }

    public async Task<User?> UserTokenAsync(string m2mToken, string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"/api/v2/users/oauth2|spotify|spotify:user:{username}", UriKind.Relative));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", m2mToken);

            var response = await _client.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();
            var userResponse = await stream.ToResponse<UserResponse>(_serializer.Value);

            return userResponse?.ToUser();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not request auth0 user access token.");
            return null;
        }
    }
}
