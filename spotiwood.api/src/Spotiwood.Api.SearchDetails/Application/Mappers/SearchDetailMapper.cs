using AutoMapper;
using SearchDetail = Spotiwood.Api.SearchDetails.Domain.SearchDetail;
using SearchDetailDto = Spotiwood.Integrations.Omdb.Domain.SearchDetail;

namespace Spotiwood.Api.SearchDetails.Application.Mappers;
internal sealed class SearchDetailMapper : Profile
{
    public SearchDetailMapper()
    {
        CreateMap<SearchDetailDto, SearchDetail>()
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
            .ForMember(target => target.Genres,
                opt => opt.MapFrom(source => source.Genres))
            .ForMember(target => target.Plot,
                opt => opt.MapFrom(source => source.Plot))
            .ForMember(target => target.Seasons,
                opt => opt.MapFrom(source => source.Seasons))
            .ForMember(target => target.Type,
                opt => opt.MapFrom(source => source.Type))
            .ForMember(target => target.Actors,
                opt => opt.MapFrom(source => source.Actors));
    }
}
