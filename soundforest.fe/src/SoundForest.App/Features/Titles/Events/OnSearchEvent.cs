using SoundForest.Framework.Api.Application.Dtos;
using SoundForest.Framework.Messaging;

namespace SoundForest.App.Features.Titles.Events;

public sealed record OnSearchEvent : DataMessageEvent<PagedCollection<TitleSummary>>
{
    public OnSearchEvent(PagedCollection<TitleSummary> data) 
        : base(data)
    {
    }
}
