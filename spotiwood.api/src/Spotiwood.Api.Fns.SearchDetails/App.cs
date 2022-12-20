using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Api.Search.Domain;
using System.Data;
using System.Net;

namespace Spotiwood.Api.Fns.SearchDetails;
public sealed class App
{
    private readonly IMediator _mediator;

    public App(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function("SearchDetail")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "search/{identifier}")] HttpRequestData request,
        string identifier,
        FunctionContext ctx)
    {
        var result = await _mediator.Send(new SearchByIdQuery(identifier));
        var response = request.CreateResponse();

        result.Switch(
            success: async (s, v) =>
            {
                await response.WriteAsJsonAsync<SearchDetail>(v);
                response.StatusCode = (HttpStatusCode)s;
            },
            failure: async (s, m, e) =>
            {
                var error = new
                {
                    Message = m,
                    Errors = e?.Select(e => e?.Message)
                };

                await response.WriteAsJsonAsync(error);
                response.StatusCode = (HttpStatusCode)s;
            });

        return response;
    }
}
