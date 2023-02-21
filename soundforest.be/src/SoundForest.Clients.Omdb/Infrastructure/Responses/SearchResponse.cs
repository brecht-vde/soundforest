namespace SoundForest.Clients.Omdb.Infrastructure.Responses;
internal sealed record SearchResultArrayResponse
{
    public IEnumerable<SearchResultResponse>? Search { get; init; }
    public string? TotalResults { get; init; }
    public int PageSize { get; init; } = 10;
    public int Page { get; set; } = 1;
    public string? Response { get; init; }
}

internal sealed record SearchResultResponse
{
    public string? Title { get; init; }
    public string? Year { get; init; }
    public string? ImdbID { get; init; }
    public string? Type { get; init; }
    public string? Poster { get; init; }
}
