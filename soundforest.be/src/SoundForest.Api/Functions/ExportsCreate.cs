using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using SoundForest.Api.Dtos;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Tools;
using System.Net;
using System.Text.Json;

namespace SoundForest.Api.Exports.Create;

public sealed class ExportsCreate
{
    private readonly IMediator _mediator;
    private readonly IOptions<JsonSerializerOptions> _options;

    public ExportsCreate(IMediator mediator, IOptions<JsonSerializerOptions> options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [Function("Exports.Create")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "exports")] HttpRequestData request,
        FunctionContext ctx)
    {
        var user = request.ExtractUserName();
        var requestBody = await JsonSerializer.DeserializeAsync<CreateExportCommandRequest>(request.Body, _options?.Value);
        var command = new CreateExportCommand(requestBody?.Id, requestBody?.Name, user);
        var result = await _mediator.Send(command);
        var response = request.CreateResponse();

        result.Switch(
            success: async (s, v) =>
            {
                await response.WriteAsJsonAsync<Export>(v);
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
