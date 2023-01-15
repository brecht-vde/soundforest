using System.Text.RegularExpressions;

namespace SoundForest.Exports.Infrastructure.Parsers;
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

    public static string? ParseTitle(this string? text)
    {
        var content = text?.Trim()?.Split('\n');
        var title = content?.ElementAtOrDefault(0)?.Trim();

        if (title?.Equals("It looks like there are no Soundtracks for this title.") == true)
            title = null;

        return title;
    }

    public static IEnumerable<string>? ParseArtists(this string? text)
    {
        var content = text?.Trim()?.Split('\n');

        if (content?.Any() != true) return new string[0];

        var artists = new List<string>();

        foreach (var detail in content.Skip(1))
        {
            var artist = _regex.Replace(detail, "").Trim();
            if (!string.IsNullOrWhiteSpace(artist) &&
                !artists.Any(a => a.Equals(artist, StringComparison.OrdinalIgnoreCase)))
            {
                artists.Add(artist);
            }
        }

        if (artists.Count() == 0 && content.Length == 2)
        {
            artists.Add(content[1].Trim());
        }

        return artists;
    }
}
