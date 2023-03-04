using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;

namespace SoundForest.Exports.Processing.Infrastructure.Parsers;
internal sealed class ImdbDocumentLoader : IDocumentLoader
{
    private readonly ILogger<ImdbDocumentLoader> _logger;
    private readonly HttpClient _client;
    private readonly HtmlParser _parser;

    public ImdbDocumentLoader(ILogger<ImdbDocumentLoader> logger, HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _parser = new HtmlParser();
    }

    public async Task<IHtmlDocument[]?> LoadDocumentsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
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

            return documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load documents.");
            return null;
        }
    }
}
