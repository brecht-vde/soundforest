using AngleSharp.Html.Dom;
using SoundForest.Exports.Processing.Domain;
using System.Text.RegularExpressions;

namespace SoundForest.Exports.Processing.Application.Parsers;
internal static class ParsingExtensions
{
    private static string[] _regexes = new[]
{
        "Written by",
        "Produced by",
        "Performed by",
        "Words and Music by",
        "Courtesy of",
        "Music by",
        "Lyrics by",
        "Written and performed by",
        "Uncredited",
        "arranged by",
        "conducted by",
        "by arrangement with",
        "under license from",
        "based on",
        "published by",
        "written & performed by",
        "Featuring",
        "Main vocals by",
        "Additional vocals by",
        "Traditional, arranged by",
        "performed by -",
        "by"
    };

    private static Regex _regex = new Regex(string.Join("|", _regexes.Select(r => $".?{r}.?")), RegexOptions.IgnoreCase);

    internal static IEnumerable<string>? ParseArtists(this IEnumerable<string>? texts)
    {
        return texts?.Select(t => _regex.Replace(t, "").Replace('&', ' ')).Distinct();
    }

    internal static IEnumerable<Soundtrack>? ParseSoundtracks(this IHtmlDocument[]? documents)
    {
        return documents?
            .SelectMany(d => d.QuerySelectorAll(".ipc-metadata-list__item"))?
            .Select(c =>
            {
                var title = c?.QuerySelector(".ipc-metadata-list-item__label")?.TextContent;
                var artists = c?.QuerySelectorAll(".ipc-html-content-inner-div")?.Select(e => e.TextContent).Where(e => !string.IsNullOrWhiteSpace(e));

                return new Soundtrack(
                    Title: title,
                    Artists: artists?.ParseArtists());
            }).Where(s => s?.Artists?.Count() > 0);
    }
}
