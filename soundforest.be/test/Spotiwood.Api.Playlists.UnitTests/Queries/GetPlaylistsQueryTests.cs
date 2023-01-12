using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using AutoMapper;
using Moq;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.UnitTests.Common;
using System.Threading.Tasks;
using System.Threading;
using System;
using Xunit;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Framework.Application.Pagination;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Spotiwood.Api.Playlists.UnitTests.Queries;
public sealed class GetPlaylistsQueryTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(GetPlaylistsQueryHandler).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistsQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        [Frozen] Mock<IMapper> mapper,
        PagedCollection<PlaylistDto> dtos,
        PagedCollection<Playlist> res,
        GetPlaylistsQuery query,
        GetPlaylistsQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(dtos);

        mapper
            .Setup(m => m.Map<PagedCollection<Playlist>>(dtos))
            .Returns(res);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<Playlist>>.SuccessResult(res));
    }

    [Theory]
    [AutoMoqData]
    internal async Task GetPlaylistByIdQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        GetPlaylistsQuery query,
        GetPlaylistsQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<PagedCollection<PlaylistDto>>());

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
        GetPlaylistsQuery query,
        GetPlaylistsQueryHandler sut)
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
