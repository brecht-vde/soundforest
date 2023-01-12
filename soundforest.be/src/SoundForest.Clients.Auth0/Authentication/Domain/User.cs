namespace SoundForest.Clients.Auth0.Authentication.Domain;
public sealed record User(
    string? Provider, 
    string? AccessToken, 
    string? RefreshToken, 
    string? Id, 
    string? Connection,
    bool IsSocial);