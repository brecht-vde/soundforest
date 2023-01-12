using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SoundForest.Exports.Application.Commands;
using SoundForest.Exports.Domain;
using SoundForest.Playlists.Application.Commands;
using SoundForest.Playlists.Domain;
using SoundForest.Schema;
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

                        if (exportEntity?.status?.Equals(Constants.Statuses.Pending, StringComparison.OrdinalIgnoreCase) is true)
                        {
                            _ = await _mediator.Send(new StartExportCommand(
                                    Id: exportEntity?.id,
                                    Name: exportEntity?.name,
                                    Username: exportEntity?.username));
                        }
                        else if (exportEntity?.status?.Equals(Constants.Statuses.Finalizing, StringComparison.OrdinalIgnoreCase) is true)
                        {
                            var playlist = new Playlist(
                                    Id: exportEntity?.id,
                                    Name: exportEntity?.name,
                                    ExternalId: exportEntity?.externalId
                                );

                            var result = await _mediator.Send(new CreatePlaylistCommand(playlist));

                            result.Switch(
                                success: async (s, v) =>
                                {
                                    _ = await _mediator.Send(new FinalizeExportCommand(exportEntity?.id, Status.Completed));
                                },
                                failure: async (s, m, e) =>
                                {
                                    _ = await _mediator.Send(new FinalizeExportCommand(exportEntity?.id, Status.Failed));
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not export playlist.");
            }
        }
    }
}
