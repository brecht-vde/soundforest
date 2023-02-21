using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Omdb.Application;
using SoundForest.Clients.Omdb.Domain;
using SoundForest.Clients.Omdb.Infrastructure.Mappings;
using SoundForest.Clients.Omdb.Infrastructure.Options;
using SoundForest.Clients.Omdb.Infrastructure.Responses;
using SoundForest.Clients.Omdb.Infrastructure.Responses.Extensions;
using SoundForest.Framework.Application.Pagination;
using System.Text.Json;

namespace SoundForest.Clients.Omdb.Infrastructure;
internal class OmdbClient : IOmdbClient
{
    private readonly ILogger<OmdbClient> _logger;
    private readonly IOptions<OmdbOptions> _options;
    private readonly HttpClient _client;
    private readonly IOptions<JsonSerializerOptions> _serializer;

    public OmdbClient(
        ILogger<OmdbClient> logger,
        IOptions<OmdbOptions> options,
        HttpClient client,
        IOptions<JsonSerializerOptions> serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options?.Value?.ApiKey))
            throw new ArgumentNullException(nameof(OmdbOptions.ApiKey));

        if (_options?.Value?.BaseAddress is null)
            throw new ArgumentNullException(nameof(OmdbOptions.BaseAddress));
    }

    public async Task<PagedCollection<SearchSummary>> ManyAsync(string? query, int? page, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"?apikey={_options.Value.ApiKey}&s={query}&page={page}", UriKind.Relative));
            var response = await _client.SendAsync(request, cancellationToken);

            if (response?.IsSuccessStatusCode is not true) return new();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (content is null || content.IsErrorResponse(_serializer.Value) is true) return new();

            var searchResultArrayResponse = content.ToResponse<SearchResultArrayResponse>(_serializer.Value);

            if (searchResultArrayResponse is null) return new();

            var summaries = searchResultArrayResponse.ToSummaryCollection(page);

            if (summaries is null) return new();

            return summaries;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Could not execute query against omdb api.");
            return new();
        }
    }

    public async Task<SearchDetail?> SingleAsync(string? id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"?apikey={_options.Value.ApiKey}&i={id}", UriKind.Relative));
            var response = await _client.SendAsync(request, cancellationToken);

            if (response?.IsSuccessStatusCode is not true) return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (content is null || content.IsErrorResponse(_serializer.Value) is true) return null;

            var searchDetailResponse = content.ToResponse<SearchDetailResponse>(_serializer.Value);

            if (searchDetailResponse is null) return null;

            var detail = searchDetailResponse.ToDetail();

            return detail;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Could not execute query against omdb api.");
            return null;
        }
    }
}
