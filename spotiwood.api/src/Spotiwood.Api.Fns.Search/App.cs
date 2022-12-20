using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Api.Search.Domain;
using Spotiwood.Framework.Application.Pagination;
using System.Net;

namespace Spotiwood.Api.Fns.Search;
public sealed class App
{
    private readonly IMediator _mediator;

    public App(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function("Search")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "search")] HttpRequestData request,
        FunctionContext ctx,
        string q,
        int? p)
    {
        var query = p.HasValue
            ? new FreeTextSearchQuery(q, p.Value)
            : new FreeTextSearchQuery(q);

        var result = await _mediator.Send(query);
        var response = request.CreateResponse();

        result.Switch(
            success: async (s, v) =>
            {
                await response.WriteAsJsonAsync<PagedCollection<SearchResult>>(v);
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
