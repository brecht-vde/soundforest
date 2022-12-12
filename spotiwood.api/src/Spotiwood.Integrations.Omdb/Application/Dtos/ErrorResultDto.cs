namespace Spotiwood.Integrations.Omdb.Application.Dtos;
internal sealed record ErrorResultDto
{
    public string? Response { get; init; }

    public string? Error { get; init; }
}
