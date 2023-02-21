using SoundForest.Clients.Omdb.Search.Domain;
using SoundForest.Framework.Application.Pagination;

namespace SoundForest.Clients.Omdb.Search.Application;
public interface IOmdbClient
{
    public Task<PagedCollection<SearchSummary>> ManyAsync(string? query, int? page, CancellationToken cancellationToken = default);

    public Task<SearchDetail?> SingleAsync(string? id, CancellationToken cancellationToken = default);
}
