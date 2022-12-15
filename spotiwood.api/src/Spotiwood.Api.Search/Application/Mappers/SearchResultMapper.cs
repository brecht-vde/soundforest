using AutoMapper;
using SearchResult = Spotiwood.Api.Search.Domain.SearchResult;
using SearchResultDto = Spotiwood.Integrations.Omdb.Domain.SearchResult;

namespace Spotiwood.Api.Search.Application.Mappers;
internal sealed class SearchResultMapper : Profile
{
    public SearchResultMapper()
    {
        CreateMap<SearchResultDto, SearchResult>()
            .ForMember(target => target.Title,
                opt => opt.MapFrom(source => source.Title))
            .ForMember(target => target.Identifier,
                opt => opt.MapFrom(source => source.Identifier))
            .ForMember(target => target.Poster,
                opt => opt.MapFrom(source => source.Poster))
            .ForMember(target => target.StartYear,
                opt => opt.MapFrom(source => source.StartYear))
            .ForMember(target => target.EndYear,
                opt => opt.MapFrom(source => source.EndYear))
            .ForMember(target => target.Type,
                opt => opt.MapFrom(target => target.Type));
    }
}
