using AutoMapper;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Pagination;

namespace Spotiwood.Api.Playlists.Application.Mappers;
internal sealed class PlaylistsMapper : Profile
{
    public PlaylistsMapper()
    {
        CreateMap<PagedCollection<PlaylistDto>, PagedCollection<Playlist>>()
            .ForMember(target => target.Total,
                opt => opt.MapFrom(source => source.Total))
            .ForMember(target => target.Page,
                opt => opt.MapFrom(source => source.Page))
            .ForMember(target => target.Size,
                opt => opt.MapFrom(source => source.Size))
            .ForMember(target => target.Items,
                opt => opt.MapFrom(source => source.Items));
    }
}
