namespace SoundForest.Clients.Auth0.Authentication.Domain;
public sealed record Token(string? AccessToken, string? Scope, int? ExpiresIn, string? TokenType);