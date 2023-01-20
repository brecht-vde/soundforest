using OneOf;
using SoundForest.Framework.Api.Application.Dtos;

namespace SoundForest.Framework.Api.Application.Abstractions;
public interface IService
{
    public Task<OneOf<Error, Playlist>> PlaylistAsync(string? identifier, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, PagedCollection<Playlist>>> PlaylistsAsync(int? page = null, int? size = null, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, TitleDetail>> TitleAsync(string? identifier, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, PagedCollection<TitleSummary>>> TitlesAsync(string? query, int? page = null, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, Export>> ExportAsync(string? identifier, CancellationToken cancellationToken = default);

    public Task<OneOf<Error, Export>> CreateExportAsync(string? identifier, string? Title, CancellationToken cancellationToken = default);
}
