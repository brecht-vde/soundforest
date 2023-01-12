using SoundForest.Exports.Application.Clients;
using SoundForest.Exports.Domain;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;

namespace SoundForest.Exports.Application.Queries;
public sealed record ExportByIdQuery(string? Id) : IResultRequest<Result<Export>>;

internal sealed class ExportByIdQueryHandler : IResultRequestHandler<ExportByIdQuery, Result<Export>>
{
    private readonly IClient _client;

    public ExportByIdQueryHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Export>> Handle(ExportByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.SingleAsync(request.Id, cancellationToken);

            if (result is null)
            {
                return Result<Export>
                    .NotFoundResult("Sorry, we did not find this export :(.");
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
