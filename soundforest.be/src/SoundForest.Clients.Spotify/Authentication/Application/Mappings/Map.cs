using SoundForest.Clients.Spotify.Authentication.Application.Responses;
using SoundForest.Clients.Spotify.Authentication.Domain;

namespace SoundForest.Clients.Spotify.Authentication.Application.Mappings;
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
