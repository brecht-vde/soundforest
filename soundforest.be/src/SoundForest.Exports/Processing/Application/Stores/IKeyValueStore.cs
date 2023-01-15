namespace SoundForest.Exports.Application.Stores;
internal interface IKeyValueStore<T>
{
    public T Find(string? key);

    public Task LoadAsync(CancellationToken cancellationToken = default);
}
