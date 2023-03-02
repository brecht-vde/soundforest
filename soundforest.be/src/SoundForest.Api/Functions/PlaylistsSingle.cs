using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SoundForest.Playlists.Management.Application.Queries;
using SoundForest.Playlists.Management.Domain;
using System.Net;

namespace SoundForest.Api.Playlists.Search.Details;
public sealed class PlaylistsSingle
{
    private readonly IMediator _mediator;

    public PlaylistsSingle(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function("Playlists.Single")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlists/{id}")] HttpRequestData request,
        FunctionContext ctx,
        string id)
    {
        var result = await _mediator.Send(new PlaylistByIdQuery(id));
        var response = request.CreateResponse();

        result.Switch(
            success: async (s, v) =>
            {
                await response.WriteAsJsonAsync<Playlist>(v);
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
