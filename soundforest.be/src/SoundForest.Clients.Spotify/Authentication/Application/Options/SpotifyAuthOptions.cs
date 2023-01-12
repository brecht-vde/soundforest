namespace SoundForest.Clients.Spotify.Authentication.Application.Options;
public sealed record SpotifyAuthOptions(
    string? ClientId,
    string? ClientSecret,
    Uri? BaseAddress);
