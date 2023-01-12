using SoundForest.Playlists.Domain;
using SoundForest.Schema.Playlists;

namespace SoundForest.Playlists.Infrastructure.Mappers;
internal static class PlaylistMapper
{
    public static Playlist ToPlaylist(this PlaylistEntity entity)
        => new Playlist(
                Id: entity?.id,
                Name: entity?.name,
                ExternalId: entity?.externalId
            );

    public static PlaylistEntity ToPlaylistEntity(this Playlist playlist)
        => new PlaylistEntity(
                id: playlist?.Id,
                name: playlist?.Name,
                externalId: playlist?.ExternalId
            );
}
