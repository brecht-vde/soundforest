using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.Application.Requests;
using SoundForest.Playlists.Management.Application.Clients;
using SoundForest.Playlists.Management.Domain;

namespace SoundForest.Playlists.Management.Application.Queries;
public sealed record PlaylistsQuery(int? Page, int? Size) : IResultRequest<Result<PagedCollection<Playlist>>>;

internal sealed class PlaylistsQueryHandler : IResultRequestHandler<PlaylistsQuery, Result<PagedCollection<Playlist>>>
{
    private readonly IClient _client;

    public PlaylistsQueryHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<PagedCollection<Playlist>>> Handle(PlaylistsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.ManyAsync(request.Page, request.Size, cancellationToken);

            if (result is null)
            {
                return Result<PagedCollection<Playlist>>
                    .NotFoundResult("Sorry, we did not find any playlists :(.");
            }

            return Result<PagedCollection<Playlist>>
                .SuccessResult(result);
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