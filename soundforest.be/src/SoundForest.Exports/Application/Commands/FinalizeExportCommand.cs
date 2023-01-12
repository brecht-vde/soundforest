using SoundForest.Exports.Application.Clients;
using SoundForest.Exports.Domain;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;

namespace SoundForest.Exports.Application.Commands;
public sealed record FinalizeExportCommand(string? Id, Status Status) : IResultRequest<Result<Export>>;

internal sealed class FinalizeExportCommandHandler : IResultRequestHandler<FinalizeExportCommand, Result<Export>>
{
    private readonly IClient _client;

    public FinalizeExportCommandHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Export>> Handle(FinalizeExportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.UpsertPropertiesAsync(
                id: request?.Id,
                properties: new Dictionary<string, object>()
                {
                    { nameof(Export.Status), request!.Status.ToString() }
                },
                cancellationToken: cancellationToken);

            if (result is null)
            {
                return Result<Export>
                    .NotFoundResult("Sorry, we could not save this export :(.");
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
