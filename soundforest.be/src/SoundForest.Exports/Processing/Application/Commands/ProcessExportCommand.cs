using SoundForest.Exports.Processing.Application.Exporters;
using SoundForest.Exports.Processing.Application.Parsers;
using SoundForest.Exports.Processing.Application.Stores;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;

namespace SoundForest.Exports.Processing.Application.Commands;
public sealed record ProcessExportCommand(string? Id, string? Name, string? Username) : IResultRequest<Result<string>>;

internal sealed class ProcessExportCommandHandler : IResultRequestHandler<ProcessExportCommand, Result<string>>
{
    private readonly IParser<IEnumerable<Soundtrack>?> _parser;
    private readonly IKeyValueStore<IEnumerable<string>?> _store;
    private readonly IExporter<IEnumerable<Soundtrack>?> _exporter;

    public ProcessExportCommandHandler(
        IParser<IEnumerable<Soundtrack>?> parser,
        IKeyValueStore<IEnumerable<string>?> store,
        IExporter<IEnumerable<Soundtrack>?> exporter)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
    }

    public async Task<Result<string>> Handle(ProcessExportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ids = _store.Find(request.Id)?.ToList() ?? new List<string>();
            ids.Insert(0, request.Id!);

            var tracks = await _parser.ParseAsync(ids, cancellationToken);

            var externalId = await _exporter.ExportAsync(
                items: tracks,
                username: request.Username!,
                name: request.Name!,
                cancellationToken: cancellationToken);

            if (string.IsNullOrWhiteSpace(externalId))
            {
                return Result<string>
                    .NotFoundResult("Sorry, could not export the playlist. :(.");
            }

            return Result<string>
                .SuccessResult(externalId);
        }
        catch (Exception ex)
        {
            return Result<string>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}