using Microsoft.Extensions.Logging;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
internal sealed class PlaylistService : IPlaylistService
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<PlaylistService> _logger;
    private readonly ISpotifyClientFactory _factory;

    private readonly IList<string> _log = new List<string>();

    public PlaylistService(ILogger<PlaylistService> logger, ITokenService tokenService, ISpotifyClientFactory factory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<string?> SaveAsync(IEnumerable<FullTrack>? items, string username, string name, CancellationToken cancellationToken)
    {
        try
        {
            if (items?.Any() is not true) return null;

            var spotifyUserToken = await _tokenService.GetUserAccessToken(username, cancellationToken);

            _logger.LogInformation("Fetched user token");

            if (string.IsNullOrWhiteSpace(spotifyUserToken)) return null;

            var userClient = _factory.Create<IUserProfileClient>(nameof(IUserProfileClient), spotifyUserToken);
            var user = await userClient.Current();

            _logger.LogInformation("Fetched user profile");

            if (string.IsNullOrWhiteSpace(user?.Id)) return null;

            var playlistClient = _factory.Create<IPlaylistsClient>(nameof(IPlaylistsClient), spotifyUserToken);
            var playlist = await playlistClient.Create(user.Id, new PlaylistCreateRequest(name));

            _logger.LogInformation("Created playlist");

            if (string.IsNullOrWhiteSpace(playlist?.Id)) return null;

            await playlistClient.ChangeDetails(playlist.Id, new PlaylistChangeDetailsRequest() { Description = $"Playlist generated with Sound Forest :)." });

            _logger.LogInformation("Items: " + items.Count());

            foreach (var batch in items.Chunk(100))
            {
                var uris = batch.Select(item => item.Uri).ToArray();
                await playlistClient.AddItems(playlist.Id, new PlaylistAddItemsRequest(uris));
            }

            _logger.LogInformation("Added items");

            return playlist.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not save tracks to Spotify.");
            return null;
        }
    }

}
