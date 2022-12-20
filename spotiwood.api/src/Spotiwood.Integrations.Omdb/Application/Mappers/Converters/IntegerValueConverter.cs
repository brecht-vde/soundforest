using AutoMapper;

namespace Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
internal sealed class IntegerValueConverter : IValueConverter<string?, int?>
{
    public int? Convert(string? source, ResolutionContext context)
    {
        return int.TryParse(source, out int result)
            ? result
            : null;
    }
}

