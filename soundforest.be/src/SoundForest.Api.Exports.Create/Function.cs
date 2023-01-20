using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace SoundForest.Api.Exports.Create;

public sealed class Function
{
    private readonly IMediator _mediator;
    private readonly IOptions<JsonSerializerOptions> _options;

    public Function(IMediator mediator, IOptions<JsonSerializerOptions> options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [Function("Exports")]
    [Authorize]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "exports")] HttpRequestData request,
        FunctionContext ctx)
    {
        // TODO: Move to extension methods
        request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values);
        var jwt = values?.FirstOrDefault()?.Replace("Bearer", "", StringComparison.OrdinalIgnoreCase)?.Trim();
        var token = new JwtSecurityToken(jwt);
        var user = token?.Subject?.Split(":")?.LastOrDefault();
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

    private record CreateExportCommandRequest
    {
        public string? Id { get; init; }

        public string? Name { get; init; }
    }
}
