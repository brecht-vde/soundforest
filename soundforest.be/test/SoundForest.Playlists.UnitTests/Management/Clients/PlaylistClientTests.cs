using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoundForest.Framework.CosmosDB.Application.Querying;
using SoundForest.Playlists.Management.Application.Options;
using SoundForest.Playlists.Management.Domain;
using SoundForest.Playlists.Management.Infrastructure.Clients;
using SoundForest.Schema.Playlists;
using SoundForest.UnitTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Playlists.UnitTests.Management.Clients;
public sealed class PlaylistClientTests
{
    [Theory]
    [AutoMoqData]
    internal async Task PlaylistClient_SingleAsync_NotNull_Returns_Playlist(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<PlaylistEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Id.Should().Be(entity.id);
            result!.ExternalId.Should().Be(entity.externalId);
            result!.Name.Should().Be(entity.name);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task PlaylistClient_SingleAsync_QueryNullResult_Returns_Null(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(It.IsAny<PlaylistEntity>());

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<PlaylistEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task PlaylistClient_SingleAsync_Throws_Returns_Null(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsValue);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<PlaylistEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task PlaylistClient_ManyAsync_NotNull_Returns_PlaylistDtos(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        List<PlaylistEntity> entities)
    {
        // Arrange
        var items = entities.AsQueryable().OrderBy(e => e.id);

        options.Setup(c => c.Value)
            .Returns(optionsValue);

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

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

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
    internal async Task PlaylistClient_ManyAsync_NoResults_Returns_Null(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsValue);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.GetItemLinqQueryable<PlaylistEntity>(
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>(),
                It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(new List<PlaylistEntity>().AsQueryable().OrderBy(e => e.id));

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

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
    internal async Task PlaylistClient_ManyAsync_Throws_Returns_Null(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value)
            .Returns(optionsValue);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>()))
            .Returns(container.Object);

        container.Setup(c => c.GetItemLinqQueryable<PlaylistEntity>(
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>(),
                It.IsAny<CosmosLinqSerializerOptions>()))
            .Throws(new Exception());

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

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
    internal async Task PlaylistClient_UpsertAsync_NotNull_Returns_Playlist(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        Playlist playlist)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.UpsertItemAsync<PlaylistEntity>(It.IsAny<PlaylistEntity>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertAsync(playlist, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Id.Should().Be(entity.id);
            result!.ExternalId.Should().Be(entity.externalId);
            result!.Name.Should().Be(entity.name);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task PlaylistClient_UpsertAsync_Throws_Returns_Null(
        ILogger<PlaylistClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<PlaylistEntity>> response,
        PlaylistEntity entity,
        Playlist playlist)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.UpsertItemAsync<PlaylistEntity>(It.IsAny<PlaylistEntity>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>();

        var sut = new PlaylistClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertAsync(playlist, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }
}
