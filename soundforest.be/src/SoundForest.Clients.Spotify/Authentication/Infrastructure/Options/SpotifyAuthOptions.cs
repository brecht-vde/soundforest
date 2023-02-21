namespace SoundForest.Clients.Spotify.Authentication.Infrastructure.Options;
public sealed record SpotifyAuthOptions(
    string? ClientId,
    string? ClientSecret,
    Uri? BaseAddress);
