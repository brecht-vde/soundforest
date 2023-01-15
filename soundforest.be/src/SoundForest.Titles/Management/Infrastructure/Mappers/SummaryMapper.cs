using SoundForest.Clients.Omdb.Domain;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Titles.Management.Domain;

namespace SoundForest.Titles.Management.Infrastructure.Mappers;
internal static class SummaryMapper
{
    public static Summary? ToSummary(this SearchSummary? source)
    {
        if (source is null) return null;

        return new Summary(
                Id: source?.Id,
                Title: source?.Title,
                Type: source?.Type,
                Poster: source?.Poster,
                StartYear: source?.StartYear,
                EndYear: source?.EndYear
            );
    }

    public static PagedCollection<Summary> ToSummaryCollection(this PagedCollection<SearchSummary>? source)
    {
        if (source is null) return new();

        var items = source.Items?.Select(i => i.ToSummary()).Where(i => i is not null);

        return new PagedCollection<Summary>()
        {
            Total = source.Total,
            Page = source.Page,
            Size = source.Size,
            Items = items is null
                    ? Enumerable.Empty<Summary>()
                    : items as IEnumerable<Summary>
        };
    }
}
