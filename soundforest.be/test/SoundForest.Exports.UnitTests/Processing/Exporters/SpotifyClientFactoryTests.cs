using FluentAssertions;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
using SoundForest.UnitTests.Common;
using SpotifyAPI.Web;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class SpotifyClientFactoryTests
{
    [Theory]
    [AutoMoqData]
    internal void SpotifyClientFactory_CreateSearch_HappyFlow(string token)
    {
        // Arrange
        var sut = new SpotifyClientFactory();

        // Act
        var result = sut.Create<ISearchClient>(nameof(ISearchClient), token);

        // Assert
        result.Should().NotBeNull().And.BeOfType<SearchClient>();
    }

    [Theory]
    [AutoMoqData]
    internal void SpotifyClientFactory_CreatePlaylist_HappyFlow(string token)
    {
        // Arrange
        var sut = new SpotifyClientFactory();

        // Act
        var result = sut.Create<IPlaylistsClient>(nameof(IPlaylistsClient), token);

        // Assert
        result.Should().NotBeNull().And.BeOfType<PlaylistsClient>();
    }

    [Theory]
    [AutoMoqData]
    internal void SpotifyClientFactory_CreateUser_HappyFlow(string token)
    {
        // Arrange
        var sut = new SpotifyClientFactory();

        // Act
        var result = sut.Create<IUserProfileClient>(nameof(IUserProfileClient), token);

        // Assert
        result.Should().NotBeNull().And.BeOfType<UserProfileClient>();
    }

    [Theory]
    [AutoMoqData]
    internal void SpotifyClientFactory_Create_Throws(string token)
    {
        // Arrange
        var sut = new SpotifyClientFactory();

        // Act
        var result = () => sut.Create<string>(token, token);

        // Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }
}
