namespace SoundForest.Exports.Processing.Application.Parsers;
internal interface IParsingService<T>
{
    public Task<T> ParseAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}
