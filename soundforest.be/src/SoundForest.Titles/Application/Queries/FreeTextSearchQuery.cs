using Microsoft.Extensions.Caching.Memory;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.Application.Requests;
using SoundForest.Titles.Application.Clients;
using SoundForest.Titles.Domain;

namespace SoundForest.Titles.Application.Queries;
public sealed record FreeTextSearchQuery(string Query, int Page = 1) : IResultRequest<Result<PagedCollection<Summary>>>;

internal sealed class FreeTextSearchQueryHandler : IResultRequestHandler<FreeTextSearchQuery, Result<PagedCollection<Summary>>>
{
    private readonly IMemoryCache _cache;
    private readonly IClient _client;

    public FreeTextSearchQueryHandler(IMemoryCache cache, IClient client)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<PagedCollection<Summary>>> Handle(FreeTextSearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cache.GetOrCreateAsync(
                key: $"{nameof(request)}:{request.Query}:{request.Page}",
                factory: async entry => await _client.ManyAsync(request.Query, request.Page, cancellationToken));

            if (result?.Items?.Any() is not true)
            {
                return Result<PagedCollection<Summary>>
                    .NotFoundResult("Sorry, your search query did not return any results :(.");
            }

            return Result<PagedCollection<Summary>>
                .SuccessResult(result);
        }
        catch (Exception ex)
        {
            return Result<PagedCollection<Summary>>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } });
        }
    }
}
