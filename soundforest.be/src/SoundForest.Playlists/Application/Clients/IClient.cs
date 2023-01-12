﻿using SoundForest.Framework.Application.Pagination;
using SoundForest.Playlists.Domain;

namespace SoundForest.Playlists.Application.Clients;
public interface IClient
{
    public Task<Playlist?> SingleAsync(string? id, CancellationToken cancellationToken = default);

    public Task<PagedCollection<Playlist>?> ManyAsync(int? page, int? size, CancellationToken cancellationToken = default);

    public Task<Playlist?> UpsertAsync(Playlist playlist, CancellationToken cancellationToken = default);
}
