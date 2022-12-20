using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Pagination;
using System.Data;
using System.Net;

namespace Spotiwood.Api.Fns.Playlists;
public sealed class App
{
    private readonly IMediator _mediator;

    public App(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function("Playlists")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlists")] HttpRequestData request,
        FunctionContext ctx,
        int? page,
        int? size)
    {
        var result = await _mediator.Send(new GetPlaylistsQuery(page, size));
        var response = request.CreateResponse();

        result.Switch(
            success: async (s, v) =>
            {
                await response.WriteAsJsonAsync<PagedCollection<Playlist>>(v);
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

