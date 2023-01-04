using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Spotiwood.Api.Playlists.Application.Commands;
using Spotiwood.Api.Playlists.Domain;
using System.Net;

namespace Spotiwood.Api.Fns.PlaylistsExport;
public sealed class App
{
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _options;

    public App(IMediator mediator, JsonSerializerOptions options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [Function("PlaylistExport")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "export")] HttpRequestData request,
        FunctionContext ctx)
    {
        var command = await JsonSerializer.DeserializeAsync<ExportPlaylistCommand>(request.Body, _options);
        var result = await _mediator.Send(command);
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
