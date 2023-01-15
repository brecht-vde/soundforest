using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;

namespace SoundForest.Exports.Management.Application.Commands;
public sealed record UpsertExportCommand(string? Id, IDictionary<string, object> Properties) : IResultRequest<Result<Export>>;

internal sealed class UpsertExportCommandHandler : IResultRequestHandler<UpsertExportCommand, Result<Export>>
{
    private readonly IClient _client;

    public UpsertExportCommandHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Export>> Handle(UpsertExportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.UpsertPropertiesAsync(
                id: request.Id,
                properties: request.Properties,
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
