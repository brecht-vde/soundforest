using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Pagination;

namespace SoundForest.Exports.Management.Application.Clients;
public interface IClient
{
    public Task<Export?> SingleAsync(string? id, CancellationToken cancellationToken = default);

    public Task<PagedCollection<Export>?> ManyAsync(int? page, int? size, CancellationToken cancellationToken = default);

    public Task<Export?> UpsertAsync(Export? entity, CancellationToken cancellationToken = default);

    public Task<Export?> UpsertPropertiesAsync(string? id, IDictionary<string, object> properties, CancellationToken cancellationToken);
}
