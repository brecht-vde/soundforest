using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SoundForest.Clients.Auth0.Authentication.Application;
using SoundForest.Clients.Auth0.Authentication.Domain;
using SoundForest.Clients.Spotify.Authentication.Application;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Exporters;
using SoundForest.UnitTests.Common;
using SpotifyAPI.Web;
using Xunit;
using AToken = SoundForest.Clients.Auth0.Authentication.Domain.Token;
using SToken = SoundForest.Clients.Spotify.Authentication.Domain.Token;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class SpotifyExporterTests
{
    private static SearchResponse ValidSearchResponse = new SearchResponse
    {
        Tracks = new Paging<FullTrack, SearchResponse>
        {
            Items = new List<FullTrack>
            {
                new FullTrack()
                {
                    Id = "FullTrackId",
                    ExternalIds = new Dictionary<string, string>()
                    {
                        { "key", "FullTrackExternalId" }
                    },
                    Uri = "https://fulltrackurl.local"
                }
            }
        }
    };

    private static FullPlaylist ValidPlaylist = new FullPlaylist()
    {
        Id = "FullPlaylistId"
    };

    [Theory]
    [AutoMoqData]
    internal async Task SoundtrackParser_HappyFlow(
            ILogger<SpotifyExporter> logger,
            [Frozen] Mock<ISpotifyAuthClient> spotifyAuthClient,
            [Frozen] Mock<IAuth0Client> auth0Client,
            [Frozen] Mock<IMemoryCache> cache,
            [Frozen] Mock<ISpotifyClientFactory> factory,
            [Frozen] Mock<IUserProfileClient> userClient,
            [Frozen] Mock<ISearchClient> searchClient,
            [Frozen] Mock<IPlaylistsClient> playlistClient,
            IEnumerable<Soundtrack?> songs,
            string username,
            string name,
            SToken stoken,
            AToken atoken,
            User user,
            PrivateUser spotifyUser
        )
    {
        // Arrange
        spotifyAuthClient
            .Setup(c => c.AccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stoken);

        spotifyAuthClient
            .Setup(c => c.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stoken);

        auth0Client
            .Setup(c => c.M2mTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(atoken);

        auth0Client
            .Setup(c => c.UserTokenAsync(It.IsAny<string>(), username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        searchClient
            .Setup(c => c.Item(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidSearchResponse);

        userClient
            .Setup(c => c.Current(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spotifyUser);

        playlistClient
            .Setup(c => c.Create(It.IsAny<string>(), It.IsAny<PlaylistCreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidPlaylist);

        factory
            .Setup(c => c.Create<ISearchClient>(It.IsAny<string>()))
            .Returns(searchClient.Object);

        factory
            .Setup(c => c.Create<IUserProfileClient>(It.IsAny<string>()))
            .Returns(userClient.Object);

        factory
            .Setup(c => c.Create<IPlaylistsClient>(It.IsAny<string>()))
            .Returns(playlistClient.Object);

        var sut = new SpotifyExporter(logger, spotifyAuthClient.Object, auth0Client.Object, cache.Object, factory.Object);

        // Act
        var result = await sut.ExportAsync(songs, username, name, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ExternalId.Should().Be(ValidPlaylist.Id);
        }
    }
}
