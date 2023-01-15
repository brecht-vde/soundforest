using SoundForest.Framework.Application.Pagination;
using SoundForest.Titles.Management.Domain;

namespace SoundForest.Titles.Management.Application.Clients;
public interface IClient
{
    public Task<PagedCollection<Summary>?> ManyAsync(string? query, int? page, CancellationToken cancellationToken = default);

    public Task<Detail?> SingleAsync(string? id, CancellationToken cancellationToken = default);
}
