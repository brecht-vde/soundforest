using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Infrastructure.Entities;
using Spotiwood.Api.Playlists.Infrastructure.Extensions;
using Spotiwood.Api.Playlists.Infrastructure.Mappers;
using Spotiwood.Api.Playlists.Infrastructure.Options;
using Spotiwood.Framework.Application.Pagination;
using System.Net;

namespace Spotiwood.Api.Playlists.Infrastructure.Clients;
internal sealed class BaseClient : IClient
{
    private readonly ILogger<BaseClient> _logger;
    private readonly IOptions<DbOptions> _options;
    private readonly ICosmosQueryBuilder _qb;
    private readonly CosmosClient _client;
    private readonly Container _container;

    private Func<int?, int?, (int, int)> PaginationDefaults = (page, size) => (page ?? 1, size ?? 10);

    public BaseClient(ILogger<BaseClient> logger, IOptions<DbOptions> options, CosmosClient client, ICosmosQueryBuilder queryBuilder)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _qb = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));

        if (options?.Value is null)
            throw new ArgumentNullException(nameof(options));

        _container = _client.GetContainer(_options.Value.Database, _options.Value.Container);
    }

    public async Task<PlaylistDto?> SingleAsync(string identifier, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<PlaylistEntity>(
                id: identifier,
                partitionKey: new PartitionKey(identifier),
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300 || response.Resource is null)
                return null;

            var dto = response.Resource.ToDto();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return null;
        }
    }

    public async Task<PagedCollection<PlaylistDto>?> ManyAsync(int? page, int? size, CancellationToken cancellationToken)
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

            var dtos = entities.Select(e => e.ToDto()).ToArray();

            var result = new PagedCollection<PlaylistDto>()
            {
                Items = dtos,
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

    public async Task<PlaylistDto?> UpsertAsync(PlaylistDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.UpsertItemAsync<PlaylistEntity>(
                item: dto.ToEntity(),
                partitionKey: new PartitionKey(dto.Identifier),
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300  || response.Resource is null)
                return null;

            dto = response.Resource.ToDto();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not upsert record.");
            return null;
        }
    }
}
