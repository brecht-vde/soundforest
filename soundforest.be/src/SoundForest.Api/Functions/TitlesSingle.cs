using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SoundForest.Titles.Management.Application.Queries;
using SoundForest.Titles.Management.Domain;
using System.Net;

namespace SoundForest.Api.Search.Details
{
    public sealed class TitlesSingle
    {
        private readonly IMediator _mediator;

        public TitlesSingle(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("Titles.Single")]
        [Authorize]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "titles/{id}")] HttpRequestData request,
            string id,
            FunctionContext ctx)
        {
            var result = await _mediator.Send(new SearchByIdQuery(id));
            var response = request.CreateResponse();

            result.Switch(
                success: async (s, v) =>
                {
                    await response.WriteAsJsonAsync<Detail>(v);
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
}