using OneOf;
using Spotiwood.Framework.Api.Application.Dtos;

namespace Spotiwood.Framework.Api.Application.Abstractions;
public interface IService
{
    public Task PlaylistDetailsAsync(string? identifier, CancellationToken cancellationToken = default);

    public Task PlaylistsAsync(int? page = null, int? size = null, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, PagedCollection<SearchResult>>> SearchAsync(string? query, int? page = null, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, SearchDetail>> SearchDetailsAsync(string? identifier, CancellationToken cancellationToken = default);
}
