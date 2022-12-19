namespace Spotiwood.Api.Playlists.Infrastructure.Entities;
internal sealed record PlaylistEntity
{
    public string? Identifier { get; init; }
    public string? Title { get; init; }
    public string? PlaylistTitle { get; init; }
    public Uri? PlaylistUri { get; init; }
}
