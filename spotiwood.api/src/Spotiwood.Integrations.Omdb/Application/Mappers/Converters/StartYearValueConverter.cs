using AutoMapper;

namespace Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
internal sealed class StartYearValueConverter : IValueConverter<string?, int?>
{
    public int? Convert(string? source, ResolutionContext context)
    {
        var years = source?.Split("–");

        return int.TryParse(years?.FirstOrDefault(), out int result)
            ? result
            : null;
    }
}
