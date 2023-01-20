using SoundForest.Framework.Messaging.State;

namespace SoundForest.App.Features.Playlists.Components.States;

internal sealed record PlaylistQueryState
{
    public int? Page { get; set; }
}

internal sealed class PlaylistQueryStateContainer : IStateContainer<PlaylistQueryState?>
{
    private PlaylistQueryState? _state;

    public PlaylistQueryState? State
    {
        get => _state;
        set => _state = value;
    }
}
