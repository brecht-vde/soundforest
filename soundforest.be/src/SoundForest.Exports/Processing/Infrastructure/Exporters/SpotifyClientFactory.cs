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

    public T Create<T>(string token)
    {
        if (_client is null)
            _client = new SpotifyClient(token);

        _logger.LogInformation($"Type: {nameof(T)}");
        _logger.LogInformation($"SC: {nameof(ISearchClient)}");
        _logger.LogInformation($"Type == SC?: {nameof(T) == nameof(ISearchClient)}");

        return nameof(T) switch
        {
            nameof(IUserProfileClient) => (T)_client.UserProfile,
            nameof(IPlaylistsClient) => (T)_client.Playlists,
            nameof(ISearchClient) => (T)_client.Search,
            _ => throw new ArgumentOutOfRangeException("Invalid type passed: " + nameof(T))
        };
    }
}
