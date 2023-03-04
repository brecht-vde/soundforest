using Microsoft.Extensions.Logging;
using SoundForest.Exports.Processing.Application.Parsers;
using SoundForest.Exports.Processing.Domain;

namespace SoundForest.Exports.Processing.Infrastructure.Parsers;
internal sealed class SoundtrackParser : IParsingService<IEnumerable<Soundtrack>?>
{
    private readonly ILogger<SoundtrackParser> _logger;
    private readonly IDocumentLoader _loader;

    public SoundtrackParser(ILogger<SoundtrackParser> logger, IDocumentLoader loader)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loader = loader ?? throw new ArgumentNullException(nameof(loader));
    }

    public async Task<IEnumerable<Soundtrack>?> ParseAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await _loader.LoadDocumentsAsync(ids, cancellationToken);

            _logger.LogInformation("documents: " + documents?.Count());

            var soundtracks = documents?.ParseSoundtracks();

            _logger.LogInformation("soundtracks: " + soundtracks?.Count());

            return soundtracks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse soundtrack documents.");
            return null;
        }
    }
}
