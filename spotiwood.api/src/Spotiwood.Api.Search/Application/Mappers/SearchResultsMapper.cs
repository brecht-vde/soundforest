using AutoMapper;
using Spotiwood.Framework.Application.Pagination;
using SearchResult = Spotiwood.Api.Search.Domain.SearchResult;
using Spotiwood.Integrations.Omdb.Domain;

namespace Spotiwood.Api.Search.Application.Mappers;
internal sealed class SearchResultsMapper : Profile
{
    public SearchResultsMapper()
    {
        CreateMap<SearchResultCollection, PagedCollection<SearchResult>>()
            .ForMember(target => target.Page,
                opt => opt.MapFrom(source => source.Page))
            .ForMember(target => target.Size,
                opt => opt.MapFrom(source => source.PageSize))
            .ForMember(target => target.Total,
                opt => opt.MapFrom(source => source.Total))
            .ForMember(target => target.Items,
                opt => opt.MapFrom(source => source.Results));
    }
}
