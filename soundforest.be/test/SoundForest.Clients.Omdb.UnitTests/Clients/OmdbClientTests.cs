using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using SoundForest.Clients.Omdb.Domain;
using SoundForest.Clients.Omdb.Infrastructure;
using SoundForest.Clients.Omdb.Infrastructure.Options;
using SoundForest.Clients.Omdb.Infrastructure.Responses;
using SoundForest.Framework.Application.Pagination;
using SoundForest.UnitTests.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Clients.Omdb.UnitTests.Clients;
public sealed class OmdbClientTests
{
    private static ErrorResponse DefaultErrorResult => new ErrorResponse() { Error = "N/A", Response = "False" };

    [Fact]
    internal void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(OmdbClient).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SingleAsync_FailedRequest_Returns_Null(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        JsonSerializerOptions serializerValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(serializerValue);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SingleAsync_ResponseTypeFalse_Returns_Null(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<ErrorResponse>(DefaultErrorResult);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SingleAsync_ResponseIsNull_Returns_Null(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<string>(null);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SingleAsync_Throws_Returns_Null(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .Throws(new Exception());

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SingleAsync_Succeeds_Returns_Valid_SearchDetailDto(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        SearchDetailResponse dto,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<SearchDetailResponse>(dto);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SearchAsync_FailedRequest_Returns_Empty_SearchResultCollection(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;


        handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.ManyAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<PagedCollection<SearchSummary>>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SearchAsync_ResponseTypeFalse_Returns_Empty_SearchResultCollection(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;


        handler.SetupAnyRequest()
            .ReturnsJsonResponse<ErrorResponse>(DefaultErrorResult);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.ManyAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<PagedCollection<SearchSummary>>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SearchAsync_ResponseIsNull_Returns_Empty_SearchResultCollection(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<string>(null);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.ManyAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<PagedCollection<SearchSummary>>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SearchAsync_Throws_Returns_Empty_SearchResultCollection(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .Throws(new Exception());

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.ManyAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<PagedCollection<SearchSummary>>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task OmdbClient_SearchAsync_Succeeds_Returns_Valid_SearchResultCollection(
        ILogger<OmdbClient> logger,
        [Frozen] Mock<IOptions<OmdbOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        OmdbOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        SearchResultArrayResponse dto,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<SearchResultArrayResponse>(dto);

        var sut = new OmdbClient(logger, options.Object, client, serializer.Object);

        // Act
        var result = await sut.ManyAsync(It.IsNotNull<string>(), dto.Page, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThan(0);
    }
}
