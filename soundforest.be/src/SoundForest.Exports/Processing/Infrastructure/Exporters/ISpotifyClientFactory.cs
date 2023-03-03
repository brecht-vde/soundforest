namespace SoundForest.Exports.Processing.Infrastructure.Exporters;
internal interface ISpotifyClientFactory
{
    public T Create<T>(string type, string token);
}
