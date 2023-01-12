namespace SoundForest.Clients.Auth0.Authentication.Application.Responses;
internal sealed record UserResponse(IList<UserIdentityResponse>? identities);

internal sealed record UserIdentityResponse(string? provider, string? access_token, string? refreshToken, string? user_id, string? connection, bool isSocial);
