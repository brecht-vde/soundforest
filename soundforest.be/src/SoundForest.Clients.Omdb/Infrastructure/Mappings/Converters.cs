namespace SoundForest.Clients.Omdb.Infrastructure.Mappings;
internal static class Converters
{
    public static int? ToStartYear(this string? value)
    {
        var years = value?.Split("–");

        return int.TryParse(years?.FirstOrDefault(), out int result)
            ? result
            : null;
    }

    public static int? ToEndYear(this string? value)
    {
        var years = value?.Split("–");

        if (years?.Length is not 2) return null;

        return int.TryParse(years.LastOrDefault(), out int result)
            ? result
            : null;
    }

    public static Uri? ToUri(this string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out Uri? uri)
            ? uri
            : null;
    }

    public static int? ToInt(this string? value)
    {
        return int.TryParse(value, out int result)
            ? result
            : null;
    }

    public static IEnumerable<string>? ToList(this string? value)
    {
        var items = value?.Split(",")?
            .Select(v => v?.Trim())?
            .Where(v => !string.IsNullOrWhiteSpace(v));

        return items?.Any() is true
            ? items as IEnumerable<string>
            : null;
    }
}
