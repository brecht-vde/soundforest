using SoundForest.Framework.Api.Application.Dtos;
using SoundForest.Framework.Messaging;

namespace SoundForest.App.Features.Playlists.Components.Events;

public sealed record OnPlaylistSearchEvent : DataMessageEvent<PagedCollection<Playlist>>
{
    public OnPlaylistSearchEvent(PagedCollection<Playlist> data)
        : base(data)
    {
    }
}
