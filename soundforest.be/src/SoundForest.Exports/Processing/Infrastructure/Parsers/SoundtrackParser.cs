using AngleSharp;
using Microsoft.Extensions.Logging;
using SoundForest.Exports.Application.Parsers;
using SoundForest.Exports.Processing.Domain;

namespace SoundForest.Exports.Infrastructure.Parsers;
internal sealed class SoundtrackParser : IParser<IEnumerable<Soundtrack>?>
{
    private readonly ILogger<SoundtrackParser> _logger;
    private readonly IBrowsingContext _context;

    public SoundtrackParser(ILogger<SoundtrackParser> logger, IBrowsingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Soundtrack>?> ParseAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var uris = ids?.Select(id => new Uri($"https://m.imdb.com/title/{id}/soundtrack"));

            if (uris?.Any() is not true) return null;

            var documents = await uris.ToAsyncEnumerable<Uri>()
                .SelectAwait(async u => await _context.OpenAsync(u.AbsoluteUri, cancellationToken))
                .ToArrayAsync();

            var containers = documents?.Select(d => d.QuerySelector(".ipl-content-list"));

            var content = containers?
                .SelectMany(c => c.Children)
                .Select(c => c.TextContent)
                .Where(tc => string.IsNullOrWhiteSpace(tc) is false);

            var soundtracks = content?.Select(c => new Soundtrack(
                    Artists: c?.ParseArtists(),
                    Title: c?.ParseTitle()
                ));

            return soundtracks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not parse soundtrack documents.");
            return null;
        }
    }
}
