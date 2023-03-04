using SpotifyAPI.Web;

namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
internal class SpotifyClientFactory : ISpotifyClientFactory
{
    // When running on Azure Functions App, type name can't be determined from T, so have to pass type name as parameter
    public T Create<T>(string type, string token)
    {
        return type switch
        {
            nameof(IUserProfileClient) => (T)new SpotifyClient(token).UserProfile,
            nameof(IPlaylistsClient) => (T)new SpotifyClient(token).Playlists,
            nameof(ISearchClient) => (T)new SpotifyClient(token).Search,
            _ => throw new ArgumentOutOfRangeException("Invalid type passed: " + nameof(T))
        };
    }
}
