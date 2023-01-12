using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Spotiwood.Api.Playlists.Infrastructure.Clients;
using Spotiwood.Api.Playlists.Infrastructure.Entities;
using Spotiwood.Api.Playlists.Infrastructure.Extensions;
using Spotiwood.Api.Playlists.Infrastructure.Options;
using Spotiwood.UnitTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        var ctor1 = () => new BaseClient(null, new Mock<IOptions<DbOptions>>().Object, new Mock<CosmosClient>().Object, new Mock<ICosmosQueryBuilder>().Object);
        var ctor2 = () => new BaseClient(new Mock<ILogger<BaseClient>>().Object, new Mock<IOptions<DbOptions>>().Object, null, new Mock<ICosmosQueryBuilder>().Object);
        var ctor3 = () => new BaseClient(new Mock<ILogger<BaseClient>>().Object, null, new Mock<CosmosClient>().Object, new Mock<ICosmosQueryBuilder>().Object);
        var ctor4 = () => new BaseClient(new Mock<ILogger<BaseClient>>().Object, new Mock<IOptions<DbOptions>>().Object, new Mock<CosmosClient>().Object, null);


        // Assert
        using (new AssertionScope())
        {
            ctor1.Should().Throw<ArgumentNullException>();
            ctor2.Should().Throw<ArgumentNullException>();
            ctor3.Should().Throw<ArgumentNullException>();
            ctor4.Should().Throw<ArgumentNullException>();
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_SingleAsync_NotNull_Returns_PlaylistDto(
        ILogger<BaseClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        DbOptions optionsInstance,
        Mock<IOptions<DbOptions>> options,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsInstance);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<PlaylistEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new BaseClient(logger, options.Object, client.Object, qb.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.PlaylistId.Should().Be(entity.playlistId);
            result!.Status.Should().Be(entity.status);
            result!.Username.Should().Be(entity.username);
            result!.Identifier.Should().Be(entity.id);
            result!.Title.Should().Be(entity.title);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_SingleAsync_QueryNullResult_Returns_Null(
        ILogger<BaseClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        DbOptions optionsInstance,
        Mock<IOptions<DbOptions>> options,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsInstance);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(It.IsAny<PlaylistEntity>());

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<PlaylistEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new BaseClient(logger, options.Object, client.Object, qb.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_SingleAsync_Throws_Returns_Null(
        ILogger<BaseClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        DbOptions optionsInstance,
        Mock<IOptions<DbOptions>> options,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsInstance);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<PlaylistEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new BaseClient(logger, options.Object, client.Object, qb.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_ManyAsync_NotNull_Returns_PlaylistDtos(
        ILogger<BaseClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        DbOptions optionsInstance,
        Mock<IOptions<DbOptions>> options,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        List<PlaylistEntity> results)
    {
        // Arrange
        var items = results.AsQueryable().OrderBy(e => e.id);

        options.Setup(c => c.Value)
            .Returns(optionsInstance);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.GetItemLinqQueryable<PlaylistEntity>(
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                It.IsAny<QueryRequestOptions>(), 
                It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(items);

        qb.Setup(q => q.ToListAsync(It.IsAny<IQueryable<PlaylistEntity>>()))
            .ReturnsAsync(items.ToList());

        var sut = new BaseClient(logger, options.Object, client.Object, qb.Object);

        // Act
        var result = await sut.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Page.Should().Be(1);
            result!.Size.Should().Be(10);
            result!.Total.Should().Be(items.Count());
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_ManyAsync_NoResults_Returns_Null(
        ILogger<BaseClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        DbOptions optionsInstance,
        Mock<IOptions<DbOptions>> options,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsInstance);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.GetItemLinqQueryable<PlaylistEntity>(
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>(),
                It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(new List<PlaylistEntity>().AsQueryable().OrderBy(e => e.id));

        var sut = new BaseClient(logger, options.Object, client.Object, qb.Object);

        // Act
        var result = await sut.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeNull();
        }
    }


    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_ManyAsync_Throws_Returns_Null(
        ILogger<BaseClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        DbOptions optionsInstance,
        Mock<IOptions<DbOptions>> options,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsInstance);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.GetItemLinqQueryable<PlaylistEntity>(
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>(),
                It.IsAny<CosmosLinqSerializerOptions>()))
            .Throws(new Exception());

        var sut = new BaseClient(logger, options.Object, client.Object, qb.Object);

        // Act
        var result = await sut.ManyAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeNull();
        }
    }
}
