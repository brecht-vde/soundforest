using SoundForest.Exports.Application.Clients;
using SoundForest.Exports.Domain;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;

namespace SoundForest.Exports.Application.Commands;
public sealed record StartExportCommand(string? Id, string? Name, string? Username) : IResultRequest<Result<Export>>;

internal sealed class StartExportCommandHandler : IResultRequestHandler<StartExportCommand, Result<Export>>
{
    private readonly IClient _client;

    public StartExportCommandHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Export>> Handle(StartExportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Status update
            var result = await _client.UpsertPropertiesAsync(
                id: request?.Id,
                properties: new Dictionary<string, object>()
                {
                    { nameof(Export.Status), nameof(Status.Running) }
                },
                cancellationToken: cancellationToken);

            // Start process
            var externalId = "";

            // Status update
            result = await _client.UpsertPropertiesAsync(
                id: request?.Id,
                properties: new Dictionary<string, object>()
                {
                    { nameof(Export.Status), nameof(Status.Finalizing) }
                },
                cancellationToken: cancellationToken);

            if (result is null)
            {
                return Result<Export>
                    .NotFoundResult("Sorry, could not export the playlist. :(.");
            }

            return Result<Export>
                .SuccessResult(result);
        }
        catch (Exception ex)
        {
            return Result<Export>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}