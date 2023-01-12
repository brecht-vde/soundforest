using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.Integrations.Omdb.Domain;
using Spotiwood.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SearchResult = Spotiwood.Api.Search.Domain.SearchResult;

namespace Spotiwood.Api.Search.UnitTests.Queries;

// TODO: Move messages to constants class
public sealed class FreeTextSearchQueryTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(FreeTextSearchQueryHandler).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task FreeTextSearchQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        [Frozen] Mock<IMapper> mapper,
        FreeTextSearchQuery query,
        SearchResultCollection dtoResults,
        PagedCollection<SearchResult> results,
        FreeTextSearchQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SearchAsync(It.IsNotNull<string>(), It.IsNotNull<int>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(dtoResults);

        mapper
            .Setup(m => m.Map<PagedCollection<SearchResult>>(It.IsNotNull<SearchResultCollection>()))
            .Returns(results);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<SearchResult>>.SuccessResult(results));
    }

    [Theory]
    [AutoMoqData]
    internal async Task FreeTextSearchQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        FreeTextSearchQuery query,
        FreeTextSearchQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SearchAsync(It.IsNotNull<string>(), It.IsNotNull<int>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<SearchResultCollection>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<SearchResult>>.NotFoundResult("Sorry, your search query did not return any results :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task FreeTextSearchQueryHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        FreeTextSearchQuery query,
        FreeTextSearchQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SearchAsync(It.IsNotNull<string>(), It.IsNotNull<int>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(500);

    }
}
