using Microsoft.Extensions.Caching.Memory;
using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;
using SoundForest.Titles.Application.Clients;
using SoundForest.Titles.Domain;

namespace SoundForest.Titles.Application.Queries;
public sealed record SearchByIdQuery(string Id) : IResultRequest<Result<Detail>>;

internal sealed class SearchByIdQueryHandler : IResultRequestHandler<SearchByIdQuery, Result<Detail>>
{
    private readonly IMemoryCache _cache;
    private readonly IClient _client;

    public SearchByIdQueryHandler(IMemoryCache cache, IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Result<Detail>> Handle(SearchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cache.GetOrCreateAsync(
                key: $"{nameof(request)}:{request.Id}",
                factory: async entry => await _client.SingleAsync(request.Id, cancellationToken));

            if (result is null)
            {
                return Result<Detail>
                    .NotFoundResult("Sorry, we did not find this title :(.");
            }

            return Result<Detail>
                .SuccessResult(result);
        }
        catch (Exception ex)
        {
            return Result<Detail>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}