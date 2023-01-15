using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Application.Mappers;
using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;

namespace SoundForest.Exports.Management.Application.Commands;
public sealed record CreateExportCommand(string? Id, string? Name, string? Username) : IResultRequest<Result<Export>>;

internal sealed class CreateExportCommandHandler : IResultRequestHandler<CreateExportCommand, Result<Export>>
{
    private readonly IClient _client;

    public CreateExportCommandHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Export>> Handle(CreateExportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.UpsertAsync(request.ToExport(), cancellationToken);
            if (result is null)
            {
                return Result<Export>
                    .NotFoundResult("Sorry, we did not find this playlist :(.");
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