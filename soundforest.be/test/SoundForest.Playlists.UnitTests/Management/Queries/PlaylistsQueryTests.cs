using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Framework.Application.Pagination;
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
public sealed class PlaylistsQueryTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(PlaylistsQueryHandler).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistsQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        PagedCollection<Playlist> playlists,
        PlaylistsQuery query,
        PlaylistsQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(playlists);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<Playlist>>.SuccessResult(playlists));
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        PlaylistsQuery query,
        PlaylistsQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<PagedCollection<Playlist>>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<Playlist>>.NotFoundResult("Sorry, we did not find any playlists :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        PlaylistsQuery query,
        PlaylistsQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(500);
        }
    }
}
