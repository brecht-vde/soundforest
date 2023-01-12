namespace SoundForest.Clients.Spotify.Authentication.Domain;
public sealed record Token(
    string? AccessToken,
    string? TokenType,
    int? ExpiresIn);
