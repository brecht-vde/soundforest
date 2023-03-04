using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
internal interface IPlaylistService
{
    public Task<string?> SaveAsync(IEnumerable<FullTrack>? items, string username, string name, CancellationToken cancellationToken);
}
