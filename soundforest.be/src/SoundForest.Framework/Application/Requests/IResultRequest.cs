using MediatR;

namespace SoundForest.Framework.Application.Requests;
public interface IResultRequest<out TResult> : IRequest<TResult>
    where TResult : IResult
{ }
