using AutoMapper;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Errors;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Framework.Application.Requests;

namespace Spotiwood.Api.Playlists.Application.Queries;
public sealed record GetPlaylistsQuery(int? Page, int? Size) : IResultRequest<Result<PagedCollection<Playlist>>>;

internal sealed class GetPlaylistsQueryHandler : IResultRequestHandler<GetPlaylistsQuery, Result<PagedCollection<Playlist>>>
{
    private readonly IClient _client;
    private readonly IMapper _mapper;

    public GetPlaylistsQueryHandler(IClient client, IMapper mapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PagedCollection<Playlist>>> Handle(GetPlaylistsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.ManyAsync(request.Page, request.Size, cancellationToken);

            if (result is null)
            {
                return Result<PagedCollection<Playlist>>
                    .NotFoundResult("Sorry, we did not find any playlists :(.");
            }

            var mapped = _mapper.Map<PagedCollection<Playlist>>(result);

            return Result<PagedCollection<Playlist>>
                .SuccessResult(mapped);
        }
        catch (Exception ex)
        {
            return Result<PagedCollection<Playlist>>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}
