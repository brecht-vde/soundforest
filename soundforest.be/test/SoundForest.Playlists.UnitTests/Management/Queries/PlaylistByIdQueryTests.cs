using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Framework.Application.Requests;
using SoundForest.Playlists.Management.Application.Clients;
using SoundForest.Playlists.Management.Application.Queries;
using SoundForest.Playlists.Management.Domain;
using SoundForest.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Playlists.UnitTests.Management.Queries;
public sealed class PlaylistByIdQueryTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(PlaylistByIdQueryHandler).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        Playlist playlist,
        PlaylistByIdQuery query,
        PlaylistByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
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
    internal async Task GetPlaylistByIdQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        PlaylistByIdQuery query,
        PlaylistByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Playlist>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Playlist>.NotFoundResult("Sorry, we did not find this playlist :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        PlaylistByIdQuery query,
        PlaylistByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
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
