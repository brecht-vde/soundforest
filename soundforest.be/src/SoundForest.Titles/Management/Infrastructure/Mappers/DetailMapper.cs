using SoundForest.Clients.Omdb.Domain;
using SoundForest.Titles.Management.Domain;

namespace SoundForest.Titles.Management.Infrastructure.Mappers;
internal static class DetailMapper
{
    public static Detail? ToDetail(this SearchDetail? source)
    {
        if (source is null) return null;

        return new Detail(
                Id: source?.Id,
                Title: source?.Title,
                StartYear: source?.StartYear,
                EndYear: source?.EndYear,
                Genres: source?.Genres,
                Plot: source?.Plot,
                Poster: source?.Poster,
                Type: source?.Type,
                Seasons: source?.Seasons,
                Actors: source?.Actors
            );
    }
}
