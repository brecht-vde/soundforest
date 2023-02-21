using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoundForest.Exports.Management.Infrastructure.Clients;
using SoundForest.Exports.Management.Infrastructure.Mappers;
using SoundForest.Exports.Management.Infrastructure.Options;
using SoundForest.Framework.CosmosDB.Application.Querying;
using SoundForest.Schema.Exports;
using SoundForest.UnitTests.Common;
using System.Net;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Clients;
public sealed class ExportClientTests
{
    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_SingleAsync_Succeeds(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        ExportEntity entity,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<ExportEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull().And.BeEquivalentTo(entity.ToExport());
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_SingleAsync_ThrowsReturnsNull(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<ExportEntity>(identifier, It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_SingleAsync_IsNotSuccessStatusCodeReturnsNull(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        string identifier)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.NotFound);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.ReadItemAsync<ExportEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_UpsertAsync_Succeeds(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        ExportEntity entity)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.UpsertItemAsync<ExportEntity>(It.IsNotNull<ExportEntity>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertAsync(entity.ToExport(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull().And.BeEquivalentTo(entity.ToExport());
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_UpsertAsync_ThrowsReturnsNull(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        ExportEntity entity)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.UpsertItemAsync<ExportEntity>(It.IsNotNull<ExportEntity>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertAsync(entity.ToExport(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_UpsertAsync_IsNotSuccessfulCodeReturnsNull(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        ExportEntity entity)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.BadRequest);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.UpsertItemAsync<ExportEntity>(It.IsNotNull<ExportEntity>(), It.IsNotNull<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertAsync(entity.ToExport(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_UpsertPropertiesAsync_Succeeds(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        ExportEntity entity,
        string identifier,
        IDictionary<string, object> properties)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
        response.Setup(r => r.Resource).Returns(entity);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.PatchItemAsync<ExportEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), It.IsNotNull<IReadOnlyList<PatchOperation>>(), It.IsAny<PatchItemRequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertPropertiesAsync(identifier, properties, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_UpsertPropertiesAsync_ThrowsReturnsNull(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        string identifier,
        IDictionary<string, object> properties)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.PatchItemAsync<ExportEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), It.IsNotNull<IReadOnlyList<PatchOperation>>(), It.IsAny<PatchItemRequestOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertPropertiesAsync(identifier, properties, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportClient_UpsertPropertiesAsync_BadRequestReturnsNull(
        ILogger<ExportClient> logger,
        Mock<ICosmosQueryBuilder> qb,
        Mock<IOptions<ClientOptions>> options,
        ClientOptions optionsValue,
        [Frozen] Mock<CosmosClient> client,
        [Frozen] Mock<Container> container,
        Mock<ItemResponse<ExportEntity>> response,
        string identifier,
        IDictionary<string, object> properties)
    {
        // Arrange
        options.Setup(c => c.Value).Returns(optionsValue);

        response.Setup(r => r.StatusCode).Returns(HttpStatusCode.BadRequest);

        client.Setup(c => c.GetContainer(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(container.Object);

        container.Setup(c => c.PatchItemAsync<ExportEntity>(It.IsNotNull<string>(), It.IsNotNull<PartitionKey>(), It.IsNotNull<IReadOnlyList<PatchOperation>>(), It.IsAny<PatchItemRequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        var sut = new ExportClient(logger, options.Object, qb.Object, client.Object);

        // Act
        var result = await sut.UpsertPropertiesAsync(identifier, properties, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }
}
