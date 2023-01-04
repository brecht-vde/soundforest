namespace Spotiwood.Framework.Api.Application.Dtos;
public sealed record SearchDetail
{
    public string? Identifier { get; init; }
    public string? Title { get; init; }
    public int? StartYear { get; init; }
    public int? EndYear { get; init; }
    public IEnumerable<string>? Genres { get; init; }
    public string? Plot { get; init; }
    public Uri? Poster { get; init; }
    public string? Type { get; init; }
    public int? Seasons { get; init; }
    public IEnumerable<string>? Actors { get; init; }
}
