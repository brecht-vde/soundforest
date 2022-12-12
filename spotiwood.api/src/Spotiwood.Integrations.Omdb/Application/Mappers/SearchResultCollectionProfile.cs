using AutoMapper;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Domain;

namespace Spotiwood.Integrations.Omdb.Application.Mappers;
internal sealed class SearchResultCollectionProfile : Profile
{
    public SearchResultCollectionProfile()
    {
        CreateMap<SearchResultDtoCollection, SearchResultCollection>()
            .ForMember(target => target.PageSize,
                opt => opt.MapFrom(source => source.PageSize))
            .ForMember(target => target.Page,
                opt => opt.MapFrom(source => source.Page))
            .ForMember(target => target.Total,
                opt => opt.MapFrom(source => source.TotalResults))
            .ForMember(target => target.Results,
                opt => opt.MapFrom(source => source.Search));
    }
}

