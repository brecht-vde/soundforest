using Spotiwood.Api.Playlists.Application.Commands;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Domain;

namespace Spotiwood.Api.Playlists.Application.Mappers;
internal static class Map
{
    public static PlaylistDto ToPlaylistDto(this ExportPlaylistCommand command)
        => new PlaylistDto()
        {
            Identifier = command.Identifier,
            Title = command.Title
        };

    public static Playlist ToPlaylist(this PlaylistDto dto)
        => new Playlist()
        {
            Identifier = dto.Identifier,
            Title = dto.Title,
            PlaylistTitle = dto.PlaylistTitle,
            PlaylistUri = dto.PlaylistUri,
            Status = dto.Status
        };
}
