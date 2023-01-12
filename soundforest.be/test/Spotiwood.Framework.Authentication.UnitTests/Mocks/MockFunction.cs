using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace Spotiwood.Framework.Authentication.UnitTests.Mocks;
public sealed class MockFunction
{
    [Function("MockFunction")]
    [Authorize]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
    string identifier)
    {
        return await Task.FromResult(req.CreateResponse());
    }
}
