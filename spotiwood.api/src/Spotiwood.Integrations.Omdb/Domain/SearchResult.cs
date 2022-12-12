namespace Spotiwood.Integrations.Omdb.Domain;
public sealed record SearchResult
{
    public string? Identifier { get; init; }
    public string? Title { get; init; }
    public int? StartYear { get; init; }
    public int? EndYear { get; init; }
    public string? Type { get; init; }
    public Uri? Poster { get; init; }
}
