using AutoMapper;

namespace Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
internal sealed class EndYearValueConverter : IValueConverter<string?, int?>
{
    public int? Convert(string? source, ResolutionContext context)
    {
        var years = source?.Split("–");

        if (years?.Length is not 2) return null;

        return int.TryParse(years.LastOrDefault(), out int result)
            ? result
            : null;
    }
}
