﻿using AutoMapper;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Api.Search.Domain;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Error = Spotiwood.Framework.Application.Errors.Error;

namespace Spotiwood.Api.Search.Application.Queries;
public sealed record FreeTextSearchQuery(string Query, int Page = 1) : IResultRequest<Result<PagedCollection<SearchResult>>>;

internal sealed class FreeTextSearchQueryHandler : IResultRequestHandler<FreeTextSearchQuery, Result<PagedCollection<SearchResult>>>
{
    private readonly IClient _client;
    private readonly IMapper _mapper;

    public FreeTextSearchQueryHandler(IClient client, IMapper mapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PagedCollection<SearchResult>>> Handle(FreeTextSearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.SearchAsync(request.Query, request.Page, cancellationToken);

            if (result?.Results?.Any() is not true)
            {
                return Result<PagedCollection<SearchResult>>
                    .NotFoundResult("Sorry, your search query did not return any results :(.");
            }

            var mapped = _mapper.Map<PagedCollection<SearchResult>>(result);

            return Result<PagedCollection<SearchResult>>
                .SuccessResult(mapped);
        }
        catch (Exception ex)
        {
            return Result<PagedCollection<SearchResult>>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}
