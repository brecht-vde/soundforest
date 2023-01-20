using SoundForest.Framework.Messaging.State;

namespace SoundForest.App.Features.Titles.States;

internal sealed record QueryState
{
    public string? Query { get; set; }

    public int? Page { get; set; }
}

internal sealed class QueryStateContainer : IStateContainer<QueryState?>
{
    private QueryState? _state;

    public QueryState? State
    {
        get => _state;
        set => _state = value;
    }
}
