using Spotiwood.Framework.Application.Errors;

namespace Spotiwood.Framework.Application.Requests;
public interface IResult
{
    public bool IsSuccessful { get; init; }
    public IEnumerable<Error>? Errors { get; init; }
    public string? DisplayMessage { get; init; }
    public int StatusCode { get; init; }
}
