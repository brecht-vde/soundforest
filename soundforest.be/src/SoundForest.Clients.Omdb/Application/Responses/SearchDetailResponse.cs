namespace SoundForest.Clients.Omdb.Application.Responses;
internal sealed record SearchDetailResponse
{
    public string? Title { get; init; }
    public string? Year { get; init; }
    public string? Rated { get; init; }
    public string? Released { get; init; }
    public string? Runtime { get; init; }
    public string? Genre { get; init; }
    public string? Director { get; init; }
    public string? Writer { get; init; }
    public string? Actors { get; init; }
    public string? Plot { get; init; }
    public string? Language { get; init; }
    public string? Country { get; init; }
    public string? Awards { get; init; }
    public string? Poster { get; init; }
    public IEnumerable<SearchDetailRatingResponse>? Ratings { get; init; }
    public string? Metascore { get; init; }
    public string? ImdbRating { get; init; }
    public string? ImdbVotes { get; init; }
    public string? ImdbID { get; init; }
    public string? Type { get; init; }
    public string? DVD { get; init; }
    public string? BoxOffice { get; init; }
    public string? Production { get; init; }
    public string? Website { get; init; }
    public string? Response { get; init; }
    public string? TotalSeasons { get; init; }
}

internal sealed record SearchDetailRatingResponse
{
    public string? Source { get; init; }
    public string? Value { get; init; }
}
