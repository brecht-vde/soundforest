using MediatR;

namespace SoundForest.Framework.Application.Requests;
public interface IResultRequestHandler<in TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : IResultRequest<TResult>
    where TResult : IResult
{ }
