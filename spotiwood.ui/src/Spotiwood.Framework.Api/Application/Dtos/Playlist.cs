namespace Spotiwood.Framework.Api.Application.Dtos;
public sealed record Playlist
{
    public string? Identifier { get; init; }

    public string? Title { get; init; }

    public string? PlaylistId { get; init; }

    public string? Status { get; init; }
}
