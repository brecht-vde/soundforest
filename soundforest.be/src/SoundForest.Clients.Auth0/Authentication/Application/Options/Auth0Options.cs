namespace SoundForest.Clients.Auth0.Authentication.Application.Options;
public sealed record Auth0Options(string? ClientId, string? ClientSecret, string? Audience, Uri? BaseAddress);
