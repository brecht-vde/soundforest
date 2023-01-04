namespace Spotiwood.Api.Playlists.Domain;
public sealed record Playlist
{
    public string? Identifier { get; init; }

    public string? Title { get; init; }

    public string? PlaylistTitle { get; init; }

    public Uri? PlaylistUri { get; init; }

    public string? Status { get; init; }
}
