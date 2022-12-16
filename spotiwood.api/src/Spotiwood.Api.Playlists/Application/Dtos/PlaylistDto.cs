namespace Spotiwood.Api.Playlists.Application.Dtos;
internal sealed record PlaylistDto
{
    public string? Identifier { get; init; }

    public string? Title { get; init; }

    public string? PlaylistTitle { get; init; }

    public Uri? PlaylistUri { get; init; }
}
