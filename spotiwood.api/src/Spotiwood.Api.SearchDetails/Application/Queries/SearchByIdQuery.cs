using AutoMapper;
using Spotiwood.Api.SearchDetails.Domain;
using Spotiwood.Framework.Application.Errors;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Integrations.Omdb.Application.Abstractions;

namespace Spotiwood.Api.SearchDetails.Application.Queries;
public sealed record SearchByIdQuery(string Identifier) : IResultRequest<Result<SearchDetail>>;

public sealed class SearchByIdQueryHandler : IResultRequestHandler<SearchByIdQuery, Result<SearchDetail>>
{
    private readonly IClient _client;
    private readonly IMapper _mapper;

    public SearchByIdQueryHandler(IClient client, IMapper mapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<SearchDetail>> Handle(SearchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.SingleAsync(request.Identifier, cancellationToken);

            if (result is null)
            {
                return Result<SearchDetail>
                    .NotFoundResult("Sorry, we did not find this title :(.");
            }

            var mapped = _mapper.Map<SearchDetail>(result);

            return Result<SearchDetail>
                .SuccessResult(mapped);
        }
        catch (Exception ex)
        {
            return Result<SearchDetail>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}