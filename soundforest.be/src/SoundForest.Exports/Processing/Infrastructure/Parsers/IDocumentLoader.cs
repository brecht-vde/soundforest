using AngleSharp.Html.Dom;

namespace SoundForest.Exports.Processing.Infrastructure.Parsers;
internal interface IDocumentLoader
{
    public Task<IHtmlDocument[]?> LoadDocumentsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}
