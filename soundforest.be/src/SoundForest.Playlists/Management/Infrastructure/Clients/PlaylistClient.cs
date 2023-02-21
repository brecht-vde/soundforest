using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.CosmosDB.Application.Querying;
using SoundForest.Playlists.Management.Application.Clients;
using SoundForest.Playlists.Management.Application.Options;
using SoundForest.Playlists.Management.Domain;
using SoundForest.Playlists.Management.Infrastructure.Mappers;
using SoundForest.Schema;
using SoundForest.Schema.Playlists;

namespace SoundForest.Playlists.Management.Infrastructure.Clients;
internal sealed class PlaylistClient : IClient
{
    private readonly ILogger<PlaylistClient> _logger;
    private readonly IOptions<ClientOptions> _options;
    private readonly ICosmosQueryBuilder _qb;
    private readonly CosmosClient _client;
    private readonly Container _container;

    private Func<int?, int?, (int, int)> PaginationDefaults = (page, size) => (page ?? 1, size ?? 10);

    public PlaylistClient(ILogger<PlaylistClient> logger, IOptions<ClientOptions> options, ICosmosQueryBuilder queryBuilder, CosmosClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _qb = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));

        if (options?.Value is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options?.Value?.ConnectionString))
            throw new ArgumentNullException(nameof(ClientOptions.ConnectionString));

        if (string.IsNullOrWhiteSpace(_options?.Value?.Database))
            throw new ArgumentNullException(nameof(ClientOptions.Database));

        _container = _client.GetContainer(_options.Value.Database, Constants.Containers.Playlists);
    }

    public async Task<PagedCollection<Playlist>?> ManyAsync(int? page, int? size, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagination = PaginationDefaults(page, size);
            var queryable = _container.GetItemLinqQueryable<PlaylistEntity>();

            var count = await queryable.CountAsync();

            if (count is null || count.Resource is 0)
                return null;

            var query = queryable
                .Skip((pagination.Item1 - 1) * pagination.Item2)
                .Take(pagination.Item2);

            var entities = await _qb.ToListAsync(query);

            if (entities is null || entities.Any() is not true)
                return null;

            var playlists = entities.Select(e => e.ToPlaylist()).ToArray();

            var result = new PagedCollection<Playlist>()
            {
                Items = playlists,
                Page = pagination.Item1,
                Size = pagination.Item2,
                Total = count.Resource
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return null;
        }
    }

    public async Task<Playlist?> SingleAsync(string? id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<PlaylistEntity>(
                id: id,
                partitionKey: new PartitionKey(id),
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300 || response.Resource is null)
                return null;

            var playlist = response?.Resource?.ToPlaylist();

            return playlist;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return null;
        }
    }

    public async Task<Playlist?> UpsertAsync(Playlist playlist, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = playlist?.ToPlaylistEntity();

            if (entity is null) return null;

            var response = await _container.UpsertItemAsync(
                item: entity,
                partitionKey: new PartitionKey(entity.id),
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300 || response.Resource is null) return null;

            return response?.Resource?.ToPlaylist();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not upsert record.");
            return null;
        }
    }
}
