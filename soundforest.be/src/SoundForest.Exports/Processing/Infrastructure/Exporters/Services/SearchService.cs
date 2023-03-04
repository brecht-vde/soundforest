using Microsoft.Extensions.Logging;
using Polly;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
internal sealed class SearchService : ISearchService
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<SearchService> _logger;
    private readonly ISpotifyClientFactory _factory;

    private readonly IList<string> _log = new List<string>();

    public SearchService(ILogger<SearchService> logger, ITokenService tokenService, ISpotifyClientFactory factory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<IEnumerable<FullTrack>?> SearchTracks(IEnumerable<Soundtrack> items, CancellationToken cancellationToken = default)
    {
        try
        {
            var spotifyM2mToken = await _tokenService.GetM2mAccesToken(cancellationToken);

            if (string.IsNullOrWhiteSpace(spotifyM2mToken)) return default;

            var searchClient = _factory.Create<ISearchClient>(nameof(ISearchClient), spotifyM2mToken);
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
                        retryCount: 3,
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                        onRetry: (ex, timespan, count, context) =>
                        {
                            _logger.LogError(ex, $"Spotify API Exception. {((APIException)ex)?.Response?.StatusCode}");
                            _logger.Log(LogLevel.Information, $"Retrying: {term}");
                        })
                        .ExecuteAndCaptureAsync(async () =>
                        {
                            var result = await searchClient.Item(new SearchRequest(SearchRequest.Types.All, term));
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
}
