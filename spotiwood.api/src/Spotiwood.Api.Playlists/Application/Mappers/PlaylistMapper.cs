using AutoMapper;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Domain;

namespace Spotiwood.Api.Playlists.Application.Mappers;
internal sealed class PlaylistMapper : Profile
{
    public PlaylistMapper()
    {
        CreateMap<PlaylistDto, Playlist>()
            .ForMember(target => target.Identifier,
                opt => opt.MapFrom(source => source.Identifier))
            .ForMember(target => target.PlaylistTitle,
                opt => opt.MapFrom(source => source.PlaylistTitle))
            .ForMember(target => target.PlaylistUri,
                opt => opt.MapFrom(source => source.PlaylistUri))
            .ForMember(target => target.Title,
                opt => opt.MapFrom(source => source.Title));
    }
}
