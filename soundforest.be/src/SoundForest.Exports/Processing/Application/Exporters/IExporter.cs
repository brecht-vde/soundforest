namespace SoundForest.Exports.Application.Exporters;
internal interface IExporter<T>
{
    Task<string?> ExportAsync(T items, string username, string name, CancellationToken cancellationToken = default);
}
