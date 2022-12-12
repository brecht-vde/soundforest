using AutoMapper;

namespace Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
internal sealed class PosterValueConverter : IValueConverter<string?, Uri?>
{
    public Uri? Convert(string? sourceMember, ResolutionContext context)
        => Uri.TryCreate(sourceMember, UriKind.Absolute, out Uri? uri)
        ? uri
        : null;
}
