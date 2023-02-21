using System.Text.RegularExpressions;

namespace SoundForest.Exports.Processing.Infrastructure.Parsers;
internal static class SountrackExtensions
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

    public static IEnumerable<string>? ParseArtists(this IEnumerable<string>? texts)
    {
        return texts?.Select(t => _regex.Replace(t, "")).Distinct();
    }
}
