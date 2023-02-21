namespace SoundForest.Clients.Auth0.Authentication.Infrastructure.Options;
public sealed record Auth0Options(string? ClientId, string? ClientSecret, string? Audience, Uri? BaseAddress);
