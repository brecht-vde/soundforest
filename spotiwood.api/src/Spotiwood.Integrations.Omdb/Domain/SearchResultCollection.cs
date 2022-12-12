namespace Spotiwood.Integrations.Omdb.Domain;
public sealed record SearchResultCollection
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 10;
    public int Total { get; init; } = 0;
    public IEnumerable<SearchResult> Results { get; init; } = Enumerable.Empty<SearchResult>();
}
