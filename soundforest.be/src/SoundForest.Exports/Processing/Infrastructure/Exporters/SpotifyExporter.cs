using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using SoundForest.Clients.Auth0.Authentication.Application;
using SoundForest.Clients.Spotify.Authentication.Application;
using SoundForest.Exports.Processing.Application.Exporters;
using SoundForest.Exports.Processing.Domain;
using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters;
internal sealed class SpotifyExporter : IExporter<IEnumerable<Soundtrack>?>
{
    private readonly ILogger<SpotifyExporter> _logger;
    private readonly ISpotifyAuthClient _spotifyAuthClient;
    private readonly IAuth0Client _auth0Client;
    private readonly IMemoryCache _cache;
    private readonly IList<string> _log = new List<string>();

    public SpotifyExporter(
        ILogger<SpotifyExporter> logger,
        ISpotifyAuthClient spotifyAuthClient,
        IAuth0Client auth0Client,
        IMemoryCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _spotifyAuthClient = spotifyAuthClient ?? throw new ArgumentNullException(nameof(spotifyAuthClient));
        _auth0Client = auth0Client ?? throw new ArgumentNullException(nameof(auth0Client));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ExportResult?> ExportAsync(IEnumerable<Soundtrack>? items, string username, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var tracks = await BuildTracksAsync(items!, cancellationToken);

            _logger.LogInformation("external tracks: " + tracks?.Count());

            var playlistId = await SaveTracksAsync(tracks, username, name, cancellationToken);

            _logger.LogInformation("playlistid: " + playlistId);

            return new ExportResult(playlistId, _log?.ToArray());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Could not export to Spotify.");
            return null;
        }
    }

    private async Task<string?> SaveTracksAsync(IEnumerable<FullTrack>? items, string username, string name, CancellationToken cancellationToken)
    {
        try
        {
            if (items?.Any() is not true) return null;

            var spotifyUserToken = await GetUserAccessToken(username, cancellationToken);

            _logger.LogInformation("Fetched user token");

            if (string.IsNullOrWhiteSpace(spotifyUserToken)) return null;

            var client = new SpotifyClient(spotifyUserToken);

            var user = await client.UserProfile.Current();

            _logger.LogInformation("Fetched user profile");

            if (string.IsNullOrWhiteSpace(user?.Id)) return null;

            var playlist = await client.Playlists.Create(user.Id, new PlaylistCreateRequest(name));

            _logger.LogInformation("Created playlist");

            if (string.IsNullOrWhiteSpace(playlist?.Id)) return null;

            await client.Playlists.ChangeDetails(playlist.Id, new PlaylistChangeDetailsRequest() { Description = $"Playlist generated with Sound Forest :)." });

            _logger.LogInformation("Items: " + items.Count());

            foreach (var batch in items.Chunk(100))
            {
                var uris = batch.Select(item => item.Uri).ToArray();
                await client.Playlists.AddItems(playlist.Id, new PlaylistAddItemsRequest(uris));
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

    private async Task<IEnumerable<FullTrack>?> BuildTracksAsync(IEnumerable<Soundtrack> items, CancellationToken cancellationToken = default)
    {
        try
        {
            var spotifyM2mToken = await GetM2mAccesToken(cancellationToken);

            if (string.IsNullOrWhiteSpace(spotifyM2mToken)) return default;

            var client = new SpotifyClient(spotifyM2mToken);
            var tracks = new List<FullTrack>();

            foreach (var item in items)
            {
                if (item?.Artists?.Any() is not true || string.IsNullOrWhiteSpace(item?.Title)) continue;

                FullTrack? foundTrack = null;

                foreach (var artist in item.Artists)
                {
                    var term = $"{item.Title} {artist}";
                    _logger.LogInformation($"Searching: {term}");
                    
                    var response = await Policy.Handle<APIException>()
                        .WaitAndRetryAsync(
                        retryCount: 4, 
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(3, attempt)), 
                        onRetry: (ex, timespan, count, context) => {
                            _logger.Log(LogLevel.Information, $"Retrying: {term}");
                        })
                        .ExecuteAndCaptureAsync(async () => { 
                            var result = await client.Search.Item(new SearchRequest(SearchRequest.Types.All, term));
                            return result;
                        });

                    var track = response?.Result?.Tracks?.Items?.FirstOrDefault();

                    if (track is null) continue;

                    if (track is not null && tracks.Any(x => x.Id == track.Id) is true) continue;

                    foundTrack = track;
                }

                if (foundTrack is null)
                {
                    _log.Add($"{item.Title} {string.Join(", ", item.Artists)}");
                    continue;
                }

                tracks.Add(foundTrack);
            }

            return tracks?.Any() is not true ? null : tracks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not find items on Spotify.");
            return default;
        }
    }

    private async Task<string?> GetUserAccessToken(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var userToken = await _cache.GetOrCreateAsync(
                key: $"spotify:auth:{username}",
                factory: async entry =>
                {
                    try
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);

                        var auth0M2mToken = await _auth0Client.M2mTokenAsync(cancellationToken);

                        if (string.IsNullOrWhiteSpace(auth0M2mToken?.AccessToken)) return null;

                        var auth0UserToken = await _auth0Client.UserTokenAsync(auth0M2mToken.AccessToken, username, cancellationToken);

                        if (string.IsNullOrWhiteSpace(auth0UserToken?.RefreshToken)) return null;

                        var spotifyUserToken = await _spotifyAuthClient.RefreshTokenAsync(auth0UserToken?.RefreshToken, cancellationToken);

                        if (string.IsNullOrWhiteSpace(spotifyUserToken?.AccessToken)) return null;

                        entry.AbsoluteExpirationRelativeToNow = spotifyUserToken?.ExpiresIn is not null
                            ? TimeSpan.FromSeconds(spotifyUserToken.ExpiresIn.Value - 300)
                            : TimeSpan.FromSeconds(1);

                        return spotifyUserToken?.AccessToken;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Could not build user access token.");
                        return null;
                    }
                });

            return userToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not acquire user access token.");
            return null;
        }
    }

    private async Task<string?> GetM2mAccesToken(CancellationToken cancellationToken = default)
    {
        try
        {
            var m2mToken = await _cache.GetOrCreateAsync(
                key: $"spotify:auth:m2m",
                factory: async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);

                    var spotifyM2mToken = await _spotifyAuthClient.AccessTokenAsync(cancellationToken);

                    if (string.IsNullOrWhiteSpace(spotifyM2mToken?.AccessToken)) return null;

                    entry.AbsoluteExpirationRelativeToNow = spotifyM2mToken?.ExpiresIn is not null
                        ? TimeSpan.FromSeconds(spotifyM2mToken.ExpiresIn.Value - 300)
                        : TimeSpan.FromSeconds(1);

                    return spotifyM2mToken?.AccessToken;
                });

            return m2mToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not acquire m2m access token.");
            return null;
        }
    }
}
