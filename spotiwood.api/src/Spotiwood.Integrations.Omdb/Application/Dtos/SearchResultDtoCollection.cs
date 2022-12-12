namespace Spotiwood.Integrations.Omdb.Application.Dtos;
internal sealed record SearchResultDtoCollection
{
    public IEnumerable<SearchResultDto>? Search { get; init; }
    public string? TotalResults { get; init; }
    public int PageSize { get; init; } = 10;
    public int Page { get; set; } = 1;
    public string? Response { get; init; }
}