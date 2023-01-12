using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.Queries;
public sealed class GetPlaylistByIdQueryTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(GetPlaylistByIdQueryHandler).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        [Frozen] Mock<IMapper> mapper,
        PlaylistDto dto,
        Playlist res,
        GetPlaylistByIdQuery query,
        GetPlaylistByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(dto);

        mapper
            .Setup(m => m.Map<Playlist>(dto))
            .Returns(res);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Playlist>.SuccessResult(res));
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        GetPlaylistByIdQuery query,
        GetPlaylistByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<PlaylistDto>());

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
        GetPlaylistByIdQuery query,
        GetPlaylistByIdQueryHandler sut)
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
