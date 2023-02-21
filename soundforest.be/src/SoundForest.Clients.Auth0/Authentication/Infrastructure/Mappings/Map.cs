using SoundForest.Clients.Auth0.Authentication.Domain;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Responses;

namespace SoundForest.Clients.Auth0.Authentication.Infrastructure.Mappings;
internal static class Map
{
    public static Token? ToToken(this TokenResponse? response)
    {
        if (response is null) return null;

        return new Token(
            AccessToken: response?.access_token,
            TokenType: response?.token_type,
            ExpiresIn: response?.expires_in,
            Scope: response?.scope);
    }

    public static User? ToUser(this UserResponse? response)
    {
        if (response is null) return null;

        var identity = response?.identities?.FirstOrDefault();

        if (identity is null) return null;

        return new User(
            Provider: identity.provider,
            AccessToken: identity.access_token,
            RefreshToken: identity.refresh_token,
            Id: identity.user_id,
            Connection: identity.connection,
            IsSocial: identity.isSocial);
    }
}
