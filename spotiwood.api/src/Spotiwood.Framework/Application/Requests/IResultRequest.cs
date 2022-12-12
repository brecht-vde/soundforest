using MediatR;

namespace Spotiwood.Framework.Application.Requests;
public interface IResultRequest<out TResult> : IRequest<TResult>
    where TResult : IResult
{ }
