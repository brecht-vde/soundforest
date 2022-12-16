using AutoMapper;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Errors;
using Spotiwood.Framework.Application.Requests;

namespace Spotiwood.Api.Playlists.Application.Queries;
public sealed record GetPlaylistByIdQuery(string Identifier) : IResultRequest<Result<Playlist>>;

internal sealed class GetPlaylistByIdQueryHandler : IResultRequestHandler<GetPlaylistByIdQuery, Result<Playlist>>
{
    private readonly IClient _client;
    private readonly IMapper _mapper;

    public GetPlaylistByIdQueryHandler(IClient client, IMapper mapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<Playlist>> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.SingleAsync(request.Identifier, cancellationToken);

            if (result is null)
            {
                return Result<Playlist>
                    .NotFoundResult("Sorry, we did not find this playlist :(.");
            }

            var mapped = _mapper.Map<Playlist>(result);

            return Result<Playlist>
                .SuccessResult(mapped);
        }
        catch (Exception ex)
        {
            return Result<Playlist>
                .ServerErrorResult(
                    message: "Whoops! Something went wrong in our system.",
                    errors: new List<Error>() { new Error() { Exception = ex } }
                );
        }
    }
}