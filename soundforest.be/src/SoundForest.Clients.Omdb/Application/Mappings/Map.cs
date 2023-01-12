using SoundForest.Clients.Omdb.Application.Responses;
using SoundForest.Clients.Omdb.Domain;
using SoundForest.Framework.Application.Pagination;

namespace SoundForest.Clients.Omdb.Application.Mappings;
internal static class Map
{
    public static SearchDetail? ToDetail(this SearchDetailResponse? response)
    {
        if (response is null) return null;

        return new SearchDetail(
            Id: response?.ImdbID,
            Title: response?.Title,
            StartYear: response?.Year?.ToStartYear(),
            EndYear: response?.Year?.ToEndYear(),
            Genres: response?.Genre?.ToList(),
            Plot: response?.Plot,
            Poster: response?.Poster.ToUri(),
            Type: response?.Type,
            Seasons: response?.TotalSeasons?.ToInt(),
            Actors: response?.Actors?.ToList());
    }

    public static SearchSummary? ToSummary(this SearchResultResponse? response)
    {
        if (response is null) return null;

        return new SearchSummary(
                Id: response?.ImdbID,
                Title: response?.Title,
                StartYear: response?.Year?.ToStartYear(),
                EndYear: response?.Year?.ToEndYear(),
                Type: response?.Type,
                Poster: response?.Poster?.ToUri());
    }

    public static PagedCollection<SearchSummary> ToSummaryCollection(this SearchResultArrayResponse? response, int? page)
    {
        var items = response?.Search?.Select(i => i?.ToSummary()).Where(i => i is not null);

        var collection = new PagedCollection<SearchSummary>()
        {
            Total = response?.TotalResults?.ToInt() ?? 0,
            Size = 10,
            Page = page ?? 1,
            Items = items is not null && items.Any() is true
                ? items as IEnumerable<SearchSummary>
                : Enumerable.Empty<SearchSummary>()
        };

        return collection;
    }
}
