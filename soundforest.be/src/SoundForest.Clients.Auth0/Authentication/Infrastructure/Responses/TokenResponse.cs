namespace SoundForest.Clients.Auth0.Authentication.Infrastructure.Responses;
internal sealed record TokenResponse(string? access_token, string? scope, int? expires_in, string? token_type);