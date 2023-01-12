using SoundForest.Framework.Application.Errors;

namespace SoundForest.Framework.Application.Requests;
public interface IResult
{
    public bool IsSuccessful { get; init; }
    public IEnumerable<Error>? Errors { get; init; }
    public string? DisplayMessage { get; init; }
    public int StatusCode { get; init; }
}
