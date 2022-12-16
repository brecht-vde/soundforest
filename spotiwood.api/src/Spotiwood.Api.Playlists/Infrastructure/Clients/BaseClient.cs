using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Infrastructure.Entities;

namespace Spotiwood.Api.Playlists.Infrastructure.Clients;
internal sealed class BaseClient : IClient
{
    private readonly ILogger<BaseClient> _logger;
    private readonly TableClient _client;

    public BaseClient(ILogger<BaseClient> logger, TableClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<PlaylistDto?> SingleAsync(string identifier, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _client.GetEntityIfExistsAsync<PlaylistEntity>(
                partitionKey: identifier,
                rowKey: identifier,
                cancellationToken: cancellationToken);

            if (entity is null || entity.HasValue is not true || entity.Value is null)
                return null;

            var dto = new PlaylistDto()
            {
                Identifier = entity.Value.PartitionKey,
                PlaylistTitle = entity.Value.PlaylistTitle,
                PlaylistUri = new Uri(entity.Value.PlaylistUri!),
                Title = entity.Value.Title,
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return null;
        }
    }
}
