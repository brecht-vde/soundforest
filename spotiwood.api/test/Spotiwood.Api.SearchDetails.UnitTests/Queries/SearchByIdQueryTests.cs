using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using Spotiwood.Api.SearchDetails.Application.Queries;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SearchDetail = Spotiwood.Api.SearchDetails.Domain.SearchDetail;
using SearchDetailDto = Spotiwood.Integrations.Omdb.Domain.SearchDetail;

namespace Spotiwood.Api.SearchDetails.UnitTests.Queries;
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
        [Frozen] Mock<IMapper> mapper,
        SearchDetailDto dto,
        SearchDetail res,
        SearchByIdQuery query,
        SearchByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(dto);

        mapper
            .Setup(m => m.Map<SearchDetail>(dto))
            .Returns(res);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<SearchDetail>.SuccessResult(res));
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
            .ReturnsAsync(It.IsAny<SearchDetailDto>());

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
