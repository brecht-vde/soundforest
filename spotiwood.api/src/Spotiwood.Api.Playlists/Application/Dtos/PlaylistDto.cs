using Newtonsoft.Json;
using Spotiwood.Api.Playlists.Domain;

namespace Spotiwood.Api.Playlists.Application.Dtos;
internal sealed record PlaylistDto
{
    [JsonProperty("id")]
    public string? Identifier { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("playlistTitle")]
    public string? PlaylistTitle { get; init; }

    [JsonProperty("playlistUri")]
    public Uri? PlaylistUri { get; init; }

    [JsonProperty("status")]
    public string? Status { get; init; }
}
