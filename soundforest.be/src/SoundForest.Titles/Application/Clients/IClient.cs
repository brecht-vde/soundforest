using SoundForest.Framework.Application.Pagination;
using SoundForest.Titles.Domain;

namespace SoundForest.Titles.Application.Clients;
public interface IClient
{
    public Task<PagedCollection<Summary>?> ManyAsync(string? query, int? page, CancellationToken cancellationToken = default);

    public Task<Detail?> SingleAsync(string? id, CancellationToken cancellationToken = default);
}
