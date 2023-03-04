using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
using SoundForest.UnitTests.Common;
using SpotifyAPI.Web;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class PlaylistServiceTests
{
    [Theory]
    [AutoMoqData]
    internal async Task PlaylistService_SearchTracks_HappyFlow(
        ILogger<PlaylistService> logger,
        Mock<ITokenService> token,
        Mock<ISpotifyClientFactory> factory,
        Mock<IUserProfileClient> userProfileClient,
        Mock<IPlaylistsClient> playlistsClient,
        IEnumerable<FullTrack> tracks,
        string username,
        string name,
        string tokenValue,
        PrivateUser user,
        FullPlaylist playlist
        )
    {
        // Arrange
        token.Setup(t => t.GetUserAccessToken(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenValue);

        userProfileClient.Setup(u => u.Current(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        playlistsClient.Setup(p => p.Create(It.IsAny<string>(), It.IsAny<PlaylistCreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        factory.Setup(f => f.Create<IUserProfileClient>(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(userProfileClient.Object);
        
        factory.Setup(f => f.Create<IPlaylistsClient>(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(playlistsClient.Object);

        var sut = new PlaylistService(logger, token.Object, factory.Object);

        // Act
        var result = await sut.SaveAsync(tracks, username, name, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Should().Be(playlist.Id);
        }
    }
}
