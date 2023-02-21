using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.Application.Requests;
using SoundForest.Titles.Management.Application.Clients;
using SoundForest.Titles.Management.Application.Queries;
using SoundForest.Titles.Management.Domain;
using SoundForest.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Titles.UnitTests.Management.Queries;
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
        PagedCollection<Summary> results,
        FreeTextSearchQuery query,
        FreeTextSearchQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.ManyAsync(It.IsNotNull<string>(), It.IsNotNull<int>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(results);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<Summary>>.SuccessResult(results));
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
            .Setup(c => c.ManyAsync(It.IsNotNull<string>(), It.IsNotNull<int>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<PagedCollection<Summary>>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<PagedCollection<Summary>>.NotFoundResult("Sorry, your search query did not return any results :(."));
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
            .Setup(c => c.ManyAsync(It.IsNotNull<string>(), It.IsNotNull<int>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(500);

    }
}
