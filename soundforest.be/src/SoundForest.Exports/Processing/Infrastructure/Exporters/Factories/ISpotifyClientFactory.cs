namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
internal interface ISpotifyClientFactory
{
    public T Create<T>(string type, string token);
}
