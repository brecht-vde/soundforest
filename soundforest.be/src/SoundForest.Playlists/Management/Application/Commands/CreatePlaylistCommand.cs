using SoundForest.Framework.Application.Errors;
using SoundForest.Framework.Application.Requests;
using SoundForest.Playlists.Management.Application.Clients;
using SoundForest.Playlists.Management.Domain;

namespace SoundForest.Playlists.Management.Application.Commands;
public sealed record CreatePlaylistCommand(Playlist Playlist) : IResultRequest<Result<Playlist>>;

internal sealed class CreatePlaylistCommandHandler : IResultRequestHandler<CreatePlaylistCommand, Result<Playlist>>
{
    private readonly IClient _client;

    public CreatePlaylistCommandHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Playlist>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.UpsertAsync(request?.Playlist!, cancellationToken);
            if (result is null)
            {
                return Result<Playlist>
                    .NotFoundResult("Sorry, we could not save this playlist :(.");
            }

            return Result<Playlist>
                .SuccessResult(result);
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
