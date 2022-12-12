namespace Spotiwood.Framework.Application.Pagination;
public sealed class PagedCollection<T>
{
    public int Page { get; init; }

    public int Size { get; init; }

    public int Total { get; init; }

    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
}
