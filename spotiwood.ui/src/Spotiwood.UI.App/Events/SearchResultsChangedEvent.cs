using OneOf;
using Spotiwood.Framework.Api.Application.Dtos;

namespace Spotiwood.UI.App.Events;

public sealed record SearchResultsChangedEvent
{
    public OneOf<Error, PagedCollection<SearchResult>> Results { get; set; }
}
