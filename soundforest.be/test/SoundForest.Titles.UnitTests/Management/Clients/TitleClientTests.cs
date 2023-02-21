using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using SoundForest.Clients.Omdb.Search.Application;
using SoundForest.Clients.Omdb.Search.Domain;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Titles.Management.Domain;
using SoundForest.Titles.Management.Infrastructure.Clients;
using SoundForest.Titles.Management.Infrastructure.Mappers;
using SoundForest.UnitTests.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Titles.UnitTests.Management.Clients;
public sealed class TitleClientTests
{
    [Theory]
    [AutoMoqData]
    internal async Task TitleClient_SingleAsync_NotNull_Returns_Detail(
        ILogger<TitleClient> logger,
        [Frozen] Mock<IOmdbClient> client,
        SearchDetail detail,
        string identifier)
    {
        // Arrange
        client.Setup(c => c.SingleAsync(identifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detail);

        var mapped = detail.ToDetail();

        var sut = new TitleClient(logger, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mapped);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task TitleClient_SingleAsync_Throws_Returns_Null(
        ILogger<TitleClient> logger,
        [Frozen] Mock<IOmdbClient> client,
        string identifier)
    {
        // Arrange
        client.Setup(c => c.SingleAsync(identifier, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new TitleClient(logger, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task TitleClient_SingleAsync_UnknownTitle_Returns_Null(
        ILogger<TitleClient> logger,
        [Frozen] Mock<IOmdbClient> client,
        string identifier)
    {
        // Arrange
        client.Setup(c => c.SingleAsync(identifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(SearchDetail));

        var sut = new TitleClient(logger, client.Object);

        // Act
        var result = await sut.SingleAsync(identifier, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task TitleClient_ManyAsync_NotNull_Returns_Summaries(
        ILogger<TitleClient> logger,
        [Frozen] Mock<IOmdbClient> client,
        PagedCollection<SearchSummary> summaries,
        string term,
        int page)
    {
        // Arrange
        client.Setup(c => c.ManyAsync(term, page, It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries);

        var sut = new TitleClient(logger, client.Object);

        // Act
        var result = await sut.ManyAsync(term, page, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Items.Count().Should().Be(summaries.Items.Count());
            result!.Page.Should().Be(summaries.Page);
            result!.Size.Should().Be(summaries.Size);
            result!.Total.Should().Be(summaries.Total);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task TitleClient_ManyAsync_Throws_Returns_Null(
        ILogger<TitleClient> logger,
        [Frozen] Mock<IOmdbClient> client,
        string term,
        int page)
    {
        // Arrange
        client.Setup(c => c.ManyAsync(term, page, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new TitleClient(logger, client.Object);

        // Act
        var result = await sut.ManyAsync(term, page, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task TitleClient_ManyAsync_UnknownTitle_Returns_EmptyCollection(
        ILogger<TitleClient> logger,
        [Frozen] Mock<IOmdbClient> client,
        string term,
        int page)
    {
        // Arrange
        client.Setup(c => c.ManyAsync(term, page, It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(PagedCollection<SearchSummary>));

        var sut = new TitleClient(logger, client.Object);

        // Act
        var result = await sut.ManyAsync(term, page, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeNull();
    }
}

