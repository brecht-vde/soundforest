namespace SoundForest.Exports.Application.Parsers;
internal interface IParser<T>
{
    public Task<T> ParseAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}
