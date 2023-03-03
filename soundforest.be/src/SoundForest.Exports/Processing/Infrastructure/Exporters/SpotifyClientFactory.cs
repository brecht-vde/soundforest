using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters;
internal class SpotifyClientFactory : ISpotifyClientFactory
{
    private SpotifyClient? _client;
    private readonly ILogger<SpotifyClientFactory> _logger;

    public SpotifyClientFactory(ILogger<SpotifyClientFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // When running on Azure Functions App, type name can't be determined from T, so have to pass type name as parameter
    public T Create<T>(string type, string token)
    {
        if (_client is null)
            _client = new SpotifyClient(token);

        return type switch
        {
            nameof(IUserProfileClient) => (T)_client.UserProfile,
            nameof(IPlaylistsClient) => (T)_client.Playlists,
            nameof(ISearchClient) => (T)_client.Search,
            _ => throw new ArgumentOutOfRangeException("Invalid type passed: " + nameof(T))
        };
    }
}
