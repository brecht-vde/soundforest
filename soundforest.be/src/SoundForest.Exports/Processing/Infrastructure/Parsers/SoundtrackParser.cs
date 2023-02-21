using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using SoundForest.Exports.Processing.Application.Parsers;
using SoundForest.Exports.Processing.Domain;

namespace SoundForest.Exports.Processing.Infrastructure.Parsers;
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

            var documents = await uris.ToAsyncEnumerable()
                .SelectAwait(async u =>
                {
                    _logger.LogInformation($"Fetching {u}.");
                    var document = await _context.OpenAsync(u.AbsoluteUri, cancellationToken);
                    await document.WaitForReadyAsync();
                    return document;
                })
                .ToArrayAsync();

            _logger.LogInformation("documents: " + documents?.Count());

            var items = documents?
                .SelectMany(d => d.QuerySelectorAll(".ipl-content-list__item"))
                .Select(c => c.TextContent)
                .Where(c => !string.IsNullOrWhiteSpace(c));

            _logger.LogInformation("items: " + items?.Count());

            var soundtracks = items?.Select(c => new Soundtrack(
                    Artists: c?.ParseArtists(),
                    Title: c?.ParseTitle()
                ));

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
