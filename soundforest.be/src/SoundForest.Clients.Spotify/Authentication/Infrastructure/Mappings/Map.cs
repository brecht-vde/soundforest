using SoundForest.Clients.Spotify.Authentication.Domain;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Responses;

namespace SoundForest.Clients.Spotify.Authentication.Infrastructure.Mappings;
internal static class Map
{
    public static Token? ToToken(this TokenResponse? response)
    {
        if (response is null) return null;

        return new Token(
            AccessToken: response?.access_token,
            TokenType: response?.token_type,
            ExpiresIn: response?.expires_in);
    }
}
