using AutoMapper;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
using Spotiwood.Integrations.Omdb.Domain;

namespace Spotiwood.Integrations.Omdb.Application.Mappers;
internal sealed class SearchResultProfile : Profile
{
    public SearchResultProfile()
    {
        CreateMap<SearchResultDto, SearchResult>()
            .ForMember(target => target.Title,
                opt => opt.MapFrom(source => source.Title))
            .ForMember(target => target.Identifier,
                opt => opt.MapFrom(source => source.ImdbID))
            .ForMember(target => target.Poster,
                opt => opt.ConvertUsing(
                    new PosterValueConverter(),
                    source => source.Poster))
            .ForMember(target => target.StartYear,
                opt => opt.ConvertUsing(
                    new StartYearValueConverter(),
                    source => source.Year))
            .ForMember(target => target.EndYear,
                opt => opt.ConvertUsing(
                    new EndYearValueConverter(),
                    source => source.Year));
    }
}