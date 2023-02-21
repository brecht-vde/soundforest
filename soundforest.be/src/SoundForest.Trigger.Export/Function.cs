using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Domain;
using SoundForest.Exports.Processing.Application.Commands;
using SoundForest.Playlists.Management.Application.Commands;
using SoundForest.Playlists.Management.Domain;
using SoundForest.Schema.Exports;

namespace SoundForest.Trigger.Export
{
    public class Function
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public Function(ILoggerFactory loggerFactory, IMediator mediator)
        {
            _logger = loggerFactory.CreateLogger<Function>();
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("ExportTrigger")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "soundforest",
            collectionName: "exports",
            ConnectionStringSetting = "SOUNDFOREST_CONNECTIONSTRING",
            LeaseConnectionStringSetting = "SOUNDFOREST_CONNECTIONSTRING",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<ExportEntity> exportEntities)
        {
            try
            {
                if (exportEntities != null && exportEntities.Count > 0)
                {
                    foreach (var exportEntity in exportEntities)
                    {
                        if (exportEntity is null)
                            continue;

                        if (Enum.TryParse<Status>(exportEntity?.status, true, out Status status) is false)
                            continue;

                        switch (status)
                        {
                            case Status.Pending:
                                await ProcessPending(exportEntity);
                                break;
                            case Status.Finalizing:
                                await ProcessFinalizing(exportEntity);
                                break;
                            case Status.NA:
                            case Status.Running:
                            case Status.Completed:
                            case Status.Failed:
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not export playlist.");
            }
        }

        private async Task ProcessFinalizing(ExportEntity exportEntity)
        {
            var playlist = new Playlist(
                Id: exportEntity?.id,
                Name: exportEntity?.name,
                ExternalId: exportEntity?.externalId
            );

            // Save playlist
            var result = await _mediator.Send(new CreatePlaylistCommand(playlist));

            // Save export status based on result
            result.Switch(
                success: async (s, v) =>
                {
                    _ = await _mediator.Send(new UpsertExportCommand(exportEntity?.id,
                        new Dictionary<string, object>() {
                                            { nameof(Exports.Management.Domain.Export.Status), Status.Completed.ToString() }
                        }));
                },
                failure: async (s, m, e) =>
                {
                    _ = await _mediator.Send(new UpsertExportCommand(exportEntity?.id,
                        new Dictionary<string, object>() {
                                            { nameof(Exports.Management.Domain.Export.Status), Status.Failed.ToString() }
                        }));
                });
        }

        private async Task ProcessPending(ExportEntity exportEntity)
        {
            // Set in progress
            _ = await _mediator.Send(new UpsertExportCommand(
                Id: exportEntity?.id,
                Properties: new Dictionary<string, object>()
                {
                    { nameof(Exports.Management.Domain.Export.Status), Status.Running.ToString() }
                }));

            // Export
            var result = await _mediator.Send(new ProcessExportCommand(
                    Id: exportEntity?.id,
                    Name: exportEntity?.name,
                    Username: exportEntity?.username));

            // Save result
            result.Switch(
                success: async (s, v) =>
                {
                    _ = await _mediator.Send(new UpsertExportCommand(
                        Id: exportEntity?.id,
                        Properties: new Dictionary<string, object>()
                        {
                            { nameof(Exports.Management.Domain.Export.Status), Status.Finalizing.ToString() },
                            { nameof(Exports.Management.Domain.Export.ExternalId), v }
                        }));
                },
                failure: async (s, m, e) =>
                {
                    _ = await _mediator.Send(new UpsertExportCommand(
                        Id: exportEntity?.id,
                        Properties: new Dictionary<string, object>()
                        {
                            { nameof(Exports.Management.Domain.Export.Status), Status.Failed.ToString() }
                        }));
                });
        }
    }
}
