namespace Spotiwood.Integrations.Omdb.Application.Dtos;
internal sealed record SearchDetailRatingDto
{
    public string? Source { get; init; }
    public string? Value { get; init; }
}
