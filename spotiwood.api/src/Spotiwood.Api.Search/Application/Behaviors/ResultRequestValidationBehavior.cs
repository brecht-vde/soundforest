using FluentValidation;
using MediatR;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Framework.Application.Errors;

namespace Spotiwood.Api.Search.Application.Behaviors;
internal sealed class ResultRequestValidationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : IResultRequest<TResult>
    where TResult : IResult, new()

{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ResultRequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var ctx = new ValidationContext<TRequest>(request);

        var results = await _validators.ToAsyncEnumerable()
            .SelectAwait(async v => await v.ValidateAsync(request, cancellationToken)).ToListAsync();

        var failures = results.SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => e.ErrorMessage);

        if (failures.Any())
        {
            var errors = failures.Select(f => new Error(f));

            return new TResult()
            {
                IsSuccessful = false,
                Errors = errors,
                DisplayMessage = "Whoops! It seems something's wrong with the request.",
                StatusCode = 400
            };
        }

        return await next();
    }
}
