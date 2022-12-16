using Azure;
using Azure.Data.Tables;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using Spotiwood.Api.Playlists.Application.Dtos;
using Spotiwood.Api.Playlists.Infrastructure.Clients;
using Spotiwood.Api.Playlists.Infrastructure.Entities;
using Spotiwood.UnitTests.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.Clients;
public sealed class BaseClientTests
{
    // TODO: rewrite with e.g. custom fixture
    [Fact]
    internal void ConstructorGuards()
    {
        // Act
        var ctor1 = () => new BaseClient(null, new Mock<TableClient>().Object);
        var ctor2 = () => new BaseClient(new Mock<ILogger<BaseClient>>().Object, null);

        // Assert
        using (new AssertionScope())
        {
            ctor1.Should().Throw<ArgumentNullException>();
            ctor2.Should().Throw<ArgumentNullException>();
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_NullValue_Returns_Null(
        ILogger<BaseClient> logger,
        Mock<TableClient> client,
        string identifier,
        Mock<NullableResponse<PlaylistEntity>> entity)
    {
        // Arrange
        client.Setup(c => c.GetEntityIfExistsAsync<PlaylistEntity>(identifier, identifier, It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity.Object);

        var sut = new BaseClient(logger, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_Throws_Returns_Null(
    ILogger<BaseClient> logger,
    Mock<TableClient> client,
    string identifier,
    Mock<NullableResponse<PlaylistEntity>> entity)
    {
        // Arrange
        client.Setup(c => c.GetEntityIfExistsAsync<PlaylistEntity>(identifier, identifier, It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var sut = new BaseClient(logger, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_NotNull_Returns_PlaylistDto(
        ILogger<BaseClient> logger,
        Mock<TableClient> client,
        string identifier,
        Mock<NullableResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        Uri uri)
    {
        // Arrange
        entity.PlaylistUri = uri.ToString();

        response.Setup(r => r.HasValue).Returns(true);
        response.Setup(r => r.Value).Returns(entity);

        client.Setup(c => c.GetEntityIfExistsAsync<PlaylistEntity>(identifier, identifier, It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new BaseClient(logger, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PlaylistDto>();
        result!.PlaylistTitle.Should().Be(entity.PlaylistTitle);
        result!.PlaylistUri.Should().Be(uri);
        result!.Identifier.Should().Be(entity.PartitionKey);
        result!.Title.Should().Be(entity.Title);
    }
}
