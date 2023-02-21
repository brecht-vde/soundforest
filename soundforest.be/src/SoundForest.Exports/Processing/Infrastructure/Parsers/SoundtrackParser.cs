using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using SoundForest.Exports.Processing.Application.Parsers;
using SoundForest.Exports.Processing.Domain;

namespace SoundForest.Exports.Processing.Infrastructure.Parsers;
internal sealed class SoundtrackParser : IParser<IEnumerable<Soundtrack>?>
{
    private readonly ILogger<SoundtrackParser> _logger;
    private readonly HttpClient _client;
    private readonly HtmlParser _parser;

    public SoundtrackParser(ILogger<SoundtrackParser> logger, HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _parser = new HtmlParser();
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
                    var req = new HttpRequestMessage(HttpMethod.Get, u);
                    var resp = await _client.SendAsync(req);
                    var html = await resp.Content.ReadAsStringAsync();
                    var document = await _parser.ParseDocumentAsync(html);
                    return document;
                })
                .ToArrayAsync();

            _logger.LogInformation("documents: " + documents?.Count());

            var soundtracks = documents?
                .SelectMany(d => d.QuerySelectorAll(".ipc-metadata-list__item"))?
                .Select(c =>
                {
                    var title = c?.QuerySelector(".ipc-metadata-list-item__label")?.TextContent;
                    var artists = c?.QuerySelectorAll(".ipc-html-content-inner-div")?.Select(e => e.TextContent).Where(e => !string.IsNullOrWhiteSpace(e));

                    return new Soundtrack(
                        Title: title,
                        Artists: artists?.ParseArtists());
                }).Where(s => s?.Artists?.Count() > 0);

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
