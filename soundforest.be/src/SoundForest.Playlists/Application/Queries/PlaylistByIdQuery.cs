using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;
using SoundForest.Playlists.Application.Clients;
using SoundForest.Playlists.Domain;

namespace SoundForest.Playlists.Application.Queries;
public sealed record PlaylistByIdQuery(string? Id) : IResultRequest<Result<Playlist>>;

internal sealed class PlaylistByIdQueryHandler : IResultRequestHandler<PlaylistByIdQuery, Result<Playlist>>
{
    private readonly IClient _client;

    public PlaylistByIdQueryHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Playlist>> Handle(PlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.SingleAsync(request.Id, cancellationToken);

            if (result is null)
            {
                return Result<Playlist>
                    .NotFoundResult("Sorry, we did not find this playlist :(.");
            }

            // Map from what?
            var mapped = result;

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