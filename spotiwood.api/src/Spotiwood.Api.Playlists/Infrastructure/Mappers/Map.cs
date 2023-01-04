using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Api.Playlists.Infrastructure.Entities;

namespace Spotiwood.Api.Playlists.Infrastructure.Mappers;
internal static class Map
{
    public static PlaylistEntity ToEntity(this PlaylistDto dto)
        => new PlaylistEntity(
            id: dto?.Identifier,
            title: dto?.Title,
            playlistTitle: dto?.PlaylistTitle,
            playlistUri: dto?.PlaylistUri?.AbsoluteUri,
            status: dto?.Status?.ToString()
            );

    public static PlaylistDto ToDto(this PlaylistEntity entity)
        => new PlaylistDto()
        {
            Identifier = entity?.id,
            Title = entity?.title,
            PlaylistTitle = entity?.playlistTitle,
            PlaylistUri = Uri.TryCreate(entity?.playlistUri, UriKind.Absolute, out Uri? uri) ? uri : null,
            Status = entity?.status
        };
}
