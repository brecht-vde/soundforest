using AutoMapper;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Application.Mappers.Converters;
using Spotiwood.Integrations.Omdb.Domain;

namespace Spotiwood.Integrations.Omdb.Application.Mappers;
internal sealed class SearchDetailProfile : Profile
{
    public SearchDetailProfile()
    {
        CreateMap<SearchDetailDto, SearchDetail>()
            .ForMember(target => target.Title,
                opt => opt.MapFrom(source => source.Title))
            .ForMember(target => target.Identifier,
                opt => opt.MapFrom(source => source.ImdbID))
            .ForMember(target => target.Type,
                opt => opt.MapFrom(source => source.Type))
            .ForMember(target => target.Poster,
                opt => opt.ConvertUsing(
                    new UriValueConverter(),
                    source => source.Poster))
            .ForMember(target => target.StartYear,
                opt => opt.ConvertUsing(
                    new StartYearValueConverter(),
                    source => source.Year))
            .ForMember(target => target.EndYear,
                opt => opt.ConvertUsing(
                    new EndYearValueConverter(),
                    source => source.Year))
            .ForMember(target => target.Plot,
                opt => opt.MapFrom(source => source.Plot))
            .ForMember(target => target.Seasons,
                opt => opt.ConvertUsing(
                    new IntegerValueConverter(),
                    source => source.TotalSeasons))
            .ForMember(target => target.Genres,
                opt => opt.ConvertUsing(
                    new CsvValueConverter(),
                    source => source.Genre))
            .ForMember(target => target.Actors,
                opt => opt.ConvertUsing(
                    new CsvValueConverter(),
                    source => source.Actors));
    }
}