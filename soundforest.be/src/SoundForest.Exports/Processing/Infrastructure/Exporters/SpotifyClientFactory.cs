using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters;
internal class SpotifyClientFactory : ISpotifyClientFactory
{
    private SpotifyClient? _client;

    public T Create<T>(string token)
    {
        if (_client is null)
            _client = new SpotifyClient(token);

        return nameof(T) switch
        {
            nameof(IUserProfileClient) => (T)_client.UserProfile,
            nameof(IPlaylistsClient) => (T)_client.Playlists,
            nameof(ISearchClient) => (T)_client.Search,
            _ => throw new ArgumentOutOfRangeException("Invalid type passed: " + nameof(T))
        };
    }
}
