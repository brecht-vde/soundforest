using Spotiwood.Framework.Application.Errors;

namespace Spotiwood.Framework.Application.Requests;
public sealed class Result<T> : IResult
{
    public T? Value { get; init; }
    public bool IsSuccessful { get; init; } = true;
    public IEnumerable<Error>? Errors { get; init; }
    public string? DisplayMessage { get; init; }
    public int StatusCode { get; init; } = 200;

    public void Switch(Action<int, T?> success, Action<int, string?, IEnumerable<Error>?> failure)
    {
        if (IsSuccessful)
            success(StatusCode, Value);
        else
            failure(StatusCode, DisplayMessage, Errors);
    }

    public static Result<T> SuccessResult(T? value)
        => new Result<T>()
        { 
            IsSuccessful = true,
            StatusCode = 200,
            Value = value
        };

    public static Result<T> NotFoundResult(string message)
        => new Result<T>()
        {
            IsSuccessful = false,
            StatusCode = 404,
            DisplayMessage = message
        };

    public static Result<T> ServerErrorResult(string message, IEnumerable<Error>? errors)
        => new Result<T>()
        {
            DisplayMessage = message,
            IsSuccessful = false,
            StatusCode = 500,
            Errors = errors
        };

    public static Result<T> UserErrorResult(string message, IEnumerable<Error>? errors)
        => new Result<T>()
        { 
            DisplayMessage = message,
            IsSuccessful = false,
            StatusCode = 400,
            Errors = errors
        };
}
