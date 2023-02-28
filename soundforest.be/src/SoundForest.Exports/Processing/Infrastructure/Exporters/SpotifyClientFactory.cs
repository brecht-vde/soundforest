using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters;
internal class SpotifyClientFactory : ISpotifyClientFactory
{
    private SpotifyClient? _client;

    public T Create<T>(string token)
    {
        if (_client is null)
            _client = new SpotifyClient(token);

        var type = typeof(T);

        switch (type)
        {
            case Type _ when type is IUserProfileClient:
                return (T)_client.UserProfile;
            case Type _ when type is IPlaylistsClient:
                return (T)_client.Playlists;
            case Type _ when type is ISearchClient:
                return (T)_client.Search;
            default:
                throw new ArgumentOutOfRangeException(nameof(T));
        }
    }
}
