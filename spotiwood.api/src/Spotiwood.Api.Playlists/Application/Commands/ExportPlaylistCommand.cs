using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Application.Mappers;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Errors;
using Spotiwood.Framework.Application.Requests;

namespace Spotiwood.Api.Playlists.Application.Commands;
public sealed record ExportPlaylistCommand(string Identifier, string Title) : IResultRequest<Result<Playlist>>;

internal sealed class ExportPlaylistCommandHandler : IResultRequestHandler<ExportPlaylistCommand, Result<Playlist>>
{
    private readonly IClient _client;

    public ExportPlaylistCommandHandler(IClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Result<Playlist>> Handle(ExportPlaylistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _client.UpsertAsync(request.ToPlaylistDto(), cancellationToken);

            if (dto is null)
            {
                return Result<Playlist>
                    .ServerErrorResult("Sorry, the playlist could not be saved.", null);
            }

            return Result<Playlist>
                .SuccessResult(dto.ToPlaylist());
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