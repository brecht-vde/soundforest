using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Framework.Application.Requests;
using SoundForest.Playlists.Management.Application.Clients;
using SoundForest.Playlists.Management.Application.Commands;
using SoundForest.Playlists.Management.Domain;
using SoundForest.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Playlists.UnitTests.Management.Commands;
public class CreatePlaylistCommandTests
{
    [Theory]
    [AutoMoqData]
    internal async Task CreatePlaylistCommandHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        Playlist playlist,
        CreatePlaylistCommand query,
        CreatePlaylistCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertAsync(It.IsNotNull<Playlist>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(playlist);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Playlist>.SuccessResult(playlist));
    }

    [Theory]
    [AutoMoqData]
    internal async Task CreatePlaylistCommandHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        CreatePlaylistCommand query,
        CreatePlaylistCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertAsync(It.IsNotNull<Playlist>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Playlist>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Playlist>.NotFoundResult("Sorry, we could not save this playlist :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task CreatePlaylistCommandHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        CreatePlaylistCommand query,
        CreatePlaylistCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertAsync(It.IsNotNull<Playlist>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
        }
    }
}
