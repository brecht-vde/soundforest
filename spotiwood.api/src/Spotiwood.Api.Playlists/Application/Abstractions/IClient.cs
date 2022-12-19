using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Framework.Application.Pagination;

namespace Spotiwood.Api.Playlists.Application.Abstractions;
internal interface IClient
{
    public Task<PlaylistDto?> SingleAsync(string identifier, CancellationToken cancellationToken);

    public Task<PagedCollection<PlaylistDto>?> ManyAsync(int? page, int? size, CancellationToken cancellationToken);
}
