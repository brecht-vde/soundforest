using Spotiwood.Api.Playlists.Application.Dtos;

namespace Spotiwood.Api.Playlists.Application.Abstractions;
internal interface IClient
{
    public Task<PlaylistDto?> SingleAsync(string identifier, CancellationToken cancellationToken);
}
