using Microsoft.Extensions.Logging;
using SoundForest.Exports.Processing.Application.Exporters;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Services;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters;
internal sealed class SpotifyExporter : IExporter<IEnumerable<Soundtrack>?>
{
    private readonly ILogger<SpotifyExporter> _logger;
    private readonly ISearchService _search;
    private readonly IPlaylistService _playlist;
    private readonly IList<string> _log = new List<string>();

    public SpotifyExporter(
        ILogger<SpotifyExporter> logger,
        ISearchService search,
        IPlaylistService playlist)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _search = search ?? throw new ArgumentNullException(nameof(search));
        _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
    }

    public async Task<ExportResult?> ExportAsync(IEnumerable<Soundtrack>? items, string username, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var tracks = await _search.SearchTracks(items!, cancellationToken);

            _logger.LogInformation("external tracks: " + tracks?.Count());

            var playlistId = await _playlist.SaveAsync(tracks, username, name, cancellationToken);

            _logger.LogInformation("playlistid: " + playlistId);

            return new ExportResult(playlistId, _log?.ToArray());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Could not export to Spotify.");
            return null;
        }
    }
}
