﻿namespace SoundForest.Exports.Processing.Application.Exporters;
internal interface IExporter<T>
{
    Task<(string?, string[]?)> ExportAsync(T items, string username, string name, CancellationToken cancellationToken = default);
}
