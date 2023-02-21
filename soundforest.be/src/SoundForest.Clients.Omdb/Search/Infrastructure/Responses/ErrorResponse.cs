namespace SoundForest.Clients.Omdb.Search.Infrastructure.Responses;
internal sealed record ErrorResponse
{
    public ErrorResponse()
    {

    }

    public string? Response { get; init; }
    public string? Error { get; init; }
}
