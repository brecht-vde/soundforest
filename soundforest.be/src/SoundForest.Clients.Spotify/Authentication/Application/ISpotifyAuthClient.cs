using SoundForest.Clients.Spotify.Authentication.Domain;

namespace SoundForest.Clients.Spotify.Authentication.Application;
public interface ISpotifyAuthClient
{
    public Task<Token?> RefreshTokenAsync(string? token, CancellationToken cancellationToken = default);

    public Task<Token?> AccessTokenAsync(CancellationToken cancellationToken = default);
}
