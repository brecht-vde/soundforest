namespace Spotiwood.Api.Playlists.Infrastructure.Entities;
internal sealed record PlaylistEntity(
    string? id,
    string? title,
    string? playlistTitle,
    string? playlistUri,
    string? status);
