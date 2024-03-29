﻿using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Domain;
using SoundForest.Exports.Management.Infrastructure.Mappers;
using SoundForest.Exports.Management.Infrastructure.Options;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.CosmosDB.Application.Querying;
using SoundForest.Schema;
using SoundForest.Schema.Exports;

namespace SoundForest.Exports.Management.Infrastructure.Clients;
internal sealed class ExportClient : IClient
{
    private readonly ILogger<ExportClient> _logger;
    private readonly IOptions<ClientOptions> _options;
    private readonly ICosmosQueryBuilder _qb;
    private readonly CosmosClient _client;
    private readonly Container _container;

    private Func<int?, int?, (int, int)> PaginationDefaults = (page, size) => (page ?? 1, size ?? 10);

    public ExportClient(ILogger<ExportClient> logger, IOptions<ClientOptions> options, ICosmosQueryBuilder queryBuilder, CosmosClient client)
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

        _container = _client.GetContainer(_options.Value.Database, Constants.Containers.Exports);
    }

    public async Task<Export?> SingleAsync(string? id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<ExportEntity>(
                id: id,
                partitionKey: new PartitionKey(id),
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300 || response.Resource is null)
                return null;

            return response?.Resource?.ToExport();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute database query.");
            return null;
        }
    }

    public async Task<Export?> UpsertAsync(Export export, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = export?.ToExportEntity();

            if (entity is null) return null;

            var response = await _container.UpsertItemAsync(
                item: entity,
                partitionKey: new PartitionKey(entity.id),
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300 || response.Resource is null) return null;

            return response?.Resource?.ToExport();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not upsert record.");
            return null;
        }
    }

    public async Task<Export?> UpsertPropertiesAsync(string? id, IDictionary<string, object> properties, CancellationToken cancellationToken)
    {
        try
        {
            var operations = properties.Select(property => PatchOperation.Replace($"/{char.ToLowerInvariant(property.Key[0]) + property.Key.Substring(1)}", property.Value)).ToList();

            var response = await _container.PatchItemAsync<ExportEntity>(
                id: id,
                partitionKey: new PartitionKey(id),
                patchOperations: operations,
                cancellationToken: cancellationToken);

            if (response is null || (int)response.StatusCode >= 300 || response.Resource is null)
                return null;

            return response?.Resource?.ToExport();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not upsert properties for record.");
            return null;
        }
    }
}
