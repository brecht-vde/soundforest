using Microsoft.Extensions.Logging;
using SoundForest.Clients.Omdb.Application;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Titles.Management.Application.Clients;
using SoundForest.Titles.Management.Domain;
using SoundForest.Titles.Management.Infrastructure.Mappers;

namespace SoundForest.Titles.Management.Infrastructure.Clients;
internal sealed class TitleClient : IClient
{
    private readonly ILogger<TitleClient> _logger;
    private readonly IOmdbClient _client;

    public TitleClient(ILogger<TitleClient> logger, IOmdbClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<PagedCollection<Summary>?> ManyAsync(string? query, int? page, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _client.ManyAsync(
                query: query,
                page: page,
                cancellationToken: cancellationToken);

            return result?.ToSummaryCollection();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute title query");
            return null;
        }
    }

    public async Task<Detail?> SingleAsync(string? id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _client.SingleAsync(
                id: id,
                cancellationToken: cancellationToken);

            return result?.ToDetail();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute title query");
            return null;
        }
    }
}
