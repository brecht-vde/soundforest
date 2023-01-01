namespace Spotiwood.Framework.Api.Application.Dtos;
public sealed record Error
{
    public string? Message { get; init; }

    public IEnumerable<string>? Errors { get; init; }
}
