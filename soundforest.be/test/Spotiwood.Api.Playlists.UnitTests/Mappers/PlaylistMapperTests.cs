using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Application.Mappers;
using Spotiwood.Api.Playlists.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.Mappers;
public sealed class PlaylistMapperTests
{
    public static IEnumerable<object[]> ValidPlaylist
    {
        get
        {
            yield return new object[]
            {
                new PlaylistDto()
                {
                    Identifier = "tt1234567",
                    Status = "status",
                    Username = "username",
                    PlaylistId = "playlistid",
                    Title = "Title"
                },
                new Playlist()
                {
                    Identifier = "tt1234567",
                    PlaylistId = "playlistid",
                    Status = "status",
                    Username = "username",
                    Title = "Title"
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(ValidPlaylist))]
    internal void SearchResultProfile_Map_Succeeds(PlaylistDto input, Playlist output)
    {
        // Arrange
        var cfg = new MapperConfiguration(cfg => cfg.AddProfile<PlaylistMapper>());
        var mapper = cfg.CreateMapper();

        // Act
        var result = mapper.Map<Playlist>(input);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<Playlist>();
            result.Should().BeEquivalentTo(output);
        }
    }
}
