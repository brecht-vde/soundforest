using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Application.Options;
using Spotiwood.Integrations.Omdb.Infrastructure.Clients;
using Spotiwood.UnitTests.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spotiwood.Integrations.Omdb.UnitTests.Clients;
public sealed class DbClientTests
{
    private static ErrorResultDto DefaultErrorResult => new ErrorResultDto() { Error = "N/A", Response = "False" };

    [Fact]
    internal void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(DbClient).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SearchAsync_FailedRequest_Returns_Empty_SearchResultCollection(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SearchAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<SearchResultDtoCollection>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SearchAsync_ResponseTypeFalse_Returns_Empty_SearchResultCollection(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<ErrorResultDto>(DefaultErrorResult);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SearchAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<SearchResultDtoCollection>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SearchAsync_ResponseIsNull_Returns_Empty_SearchResultCollection(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<string>(null);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SearchAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<SearchResultDtoCollection>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SearchAsync_Throws_Returns_Empty_SearchResultCollection(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .Throws(new Exception());

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SearchAsync(It.IsNotNull<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<SearchResultDtoCollection>(new());
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SearchAsync_Succeeds_Returns_Valid_SearchResultCollection(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        SearchResultDtoCollection dto,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<SearchResultDtoCollection>(dto);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SearchAsync(It.IsNotNull<string>(), dto.Page, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<SearchResultDtoCollection>(dto);
    }









    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SingleAsync_FailedRequest_Returns_Null(
    ILogger<DbClient> logger,
    [Frozen] Mock<IOptions<ClientOptions>> options,
    Mock<HttpMessageHandler> handler,
    ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SingleAsync_ResponseTypeFalse_Returns_Null(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<ErrorResultDto>(DefaultErrorResult);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SingleAsync_ResponseIsNull_Returns_Null(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<string>(null);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SingleAsync_Throws_Returns_Null(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .Throws(new Exception());

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task DbClient_SingleAsync_Succeeds_Returns_Valid_SearchDetailDto(
        ILogger<DbClient> logger,
        [Frozen] Mock<IOptions<ClientOptions>> options,
        Mock<HttpMessageHandler> handler,
        SearchDetailDto dto,
        ClientOptions value)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(value);

        var client = handler.CreateClient();

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<SearchDetailDto>(dto);

        var sut = new DbClient(logger, options.Object, client);

        // Act
        var result = await sut.SingleAsync(It.IsNotNull<string>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<SearchDetailDto>(dto);
    }
}
