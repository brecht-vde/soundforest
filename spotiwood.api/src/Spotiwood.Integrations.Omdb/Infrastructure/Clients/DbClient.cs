using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Application.Options;
using Spotiwood.Integrations.Omdb.Application.Serialization;
using System.Text.Json;

namespace Spotiwood.Integrations.Omdb.Infrastructure.Clients;
internal sealed class DbClient : IDbClient
{
    private readonly ILogger<DbClient> _logger;
    private readonly IOptions<ClientOptions> _options;
    private readonly HttpClient _client;

    public DbClient(
        ILogger<DbClient> logger,
        IOptions<ClientOptions> options,
        HttpClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));

        if (client.BaseAddress is null)
            client.BaseAddress = _options.Value.Uri;
    }

    public async Task<SearchResultDtoCollection> SearchAsync(string query, int page, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"?apikey={_options.Value.Key}&s={query}&page={page}", UriKind.Relative));
            var response = await _client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new();

            var content = await response.Content.ReadAsStringAsync();

            if (IsErrorResponse(content))
                return new();

            var result = JsonSerializer.Deserialize<SearchResultDtoCollection>(content, Serialization.DefaultOptions);

            if (result is null)
                return new();

            result.Page = page;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return new();
        }
    }

    public async Task<SearchDetailDto?> SingleAsync(string identifier, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"?apikey={_options.Value.Key}&i={identifier}", UriKind.Relative));
            var response = await _client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();

            if (IsErrorResponse(content))
                return null;

            var result = JsonSerializer.Deserialize<SearchDetailDto>(content, Serialization.DefaultOptions);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return null;
        }
    }

    private bool IsErrorResponse(string content)
    {
        var errorResponse = JsonSerializer.Deserialize<ErrorResultDto>(content, Serialization.DefaultOptions);
        return errorResponse?.Response?.Equals("false", StringComparison.OrdinalIgnoreCase) is true;
    }
}
