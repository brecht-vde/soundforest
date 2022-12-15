using AutoMapper;

namespace Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
internal sealed class CsvValueConverter : IValueConverter<string?, IEnumerable<string>?>
{
    public IEnumerable<string>? Convert(string? source, ResolutionContext context)
    {
        var items = source?.Split(",")?
            .Select(v => v?.Trim())?
            .Where(v => !string.IsNullOrWhiteSpace(v));

        return items?.Any() is true
            ? items as IEnumerable<string>
            : null;
    }
}
