using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Titles.Management.Application.Queries;
using SoundForest.Titles.Management.Domain;
using System.Net;

namespace SoundForest.Api.Search.Summaries
{
    public class Function
    {
        private readonly IMediator _mediator;

        public Function(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("Search")]
        [Authorize]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "titles")] HttpRequestData request,
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
                    await response.WriteAsJsonAsync<PagedCollection<Summary>>(v);
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
