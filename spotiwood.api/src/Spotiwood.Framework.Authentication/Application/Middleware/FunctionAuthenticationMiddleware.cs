using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Framework.Authentication.Application.Abstractions;
using System.Net;

namespace Spotiwood.Framework.Authentication.Application.Middleware;
public sealed class FunctionAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IAuthenticator _authenticator;

    public FunctionAuthenticationMiddleware(IAuthenticator authenticator)
    {
        _authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var data = await context.GetHttpRequestDataAsync().ConfigureAwait(false);
        IEnumerable<string>? values = null;
        data?.Headers.TryGetValues("Authorization", out values);
        var result = await _authenticator.ValidateAsync(values?.FirstOrDefault(), new CancellationTokenSource().Token);

        if (!result)
        {
            var request = await context.GetHttpRequestDataAsync().ConfigureAwait(false);
            var response = request!.CreateResponse(HttpStatusCode.Unauthorized);
            await response.WriteAsJsonAsync(Result<string>.UnauthorizedResult("Authorization header missing or invalid."), HttpStatusCode.Unauthorized).ConfigureAwait(false);
            context.GetInvocationResult().Value = response;
            return;
        }

        await next(context).ConfigureAwait(false);
    }
}