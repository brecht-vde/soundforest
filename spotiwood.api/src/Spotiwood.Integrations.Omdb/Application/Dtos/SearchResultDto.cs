namespace Spotiwood.Integrations.Omdb.Application.Dtos;
internal sealed record SearchResultDto
{
    public string? Title { get; init; }
    public string? Year { get; init; }
    public string? ImdbID { get; init; }
    public string? Type { get; init; }
    public string? Poster { get; init; }
}
