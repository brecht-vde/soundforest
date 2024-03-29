using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Playlists.Management.Application.Queries;
using SoundForest.Playlists.Management.Domain;
using System.Net;

namespace SoundForest.Api.Playlists.Search.DetailsCollection;
public sealed class PlaylistsMany
{
    private readonly IMediator _mediator;

    public PlaylistsMany(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function("Playlists.Many")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlists")] HttpRequestData request,
        FunctionContext ctx,
        int? page,
        int? size)
    {
        var result = await _mediator.Send(new PlaylistsQuery(page, size));
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
