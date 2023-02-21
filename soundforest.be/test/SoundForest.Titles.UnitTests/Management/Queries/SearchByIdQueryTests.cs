using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using SoundForest.Clients.Omdb.Domain;
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
public sealed class SearchByIdQueryTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(SearchByIdQueryHandler).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task FreeTextSearchQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        Detail res,
        SearchByIdQuery query,
        SearchByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(res);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Detail>.SuccessResult(res));
    }

    [Theory]
    [AutoMoqData]
    internal async Task FreeTextSearchQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        SearchByIdQuery query,
        SearchByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Detail>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<SearchDetail>.NotFoundResult("Sorry, we did not find this title :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task FreeTextSearchQueryHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        SearchByIdQuery query,
        SearchByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(500);
    }
}

