namespace SoundForest.Exports.Processing.Application.Parsers;
internal interface IParser<T>
{
    public Task<T> ParseAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}
