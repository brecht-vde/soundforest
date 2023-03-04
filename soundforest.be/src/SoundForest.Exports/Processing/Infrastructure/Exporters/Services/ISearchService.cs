using SoundForest.Exports.Processing.Domain;
using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
internal interface ISearchService
{
    public Task<IEnumerable<FullTrack>?> SearchTracks(IEnumerable<Soundtrack> items, CancellationToken cancellationToken = default);
}
