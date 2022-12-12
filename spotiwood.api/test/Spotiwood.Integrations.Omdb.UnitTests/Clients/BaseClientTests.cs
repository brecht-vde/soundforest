using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.Integrations.Omdb.Application.Clients;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Domain;
using Spotiwood.Integrations.Omdb.Infrastructure.Clients;
using Spotiwood.UnitTests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spotiwood.Integrations.Omdb.UnitTests.Clients;

public sealed class BaseClientTests
{
    [Fact]
    internal void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(BaseClient).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_SearchAsync_Succeeds(
        [Frozen] Mock<IDbClient> client,
        [Frozen] Mock<IMapper> mapper,
        SearchResultDtoCollection dto,
        SearchResultCollection entity,
        BaseClient sut)
    {
        // Arrange
        client
            .Setup(c => c.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        mapper
            .Setup(m => m.Map<SearchResultCollection>(dto))
            .Returns(entity);

        // Act
        var result = await sut.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull()
            .And
            .BeEquivalentTo(entity);
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_SearchAsync_Mapping_Fails_Returns_EmptyCollection(
        [Frozen] Mock<IDbClient> client,
        [Frozen] Mock<IMapper> mapper,
        SearchResultDtoCollection dto,
        BaseClient sut)
    {
        // Arrange
        client
            .Setup(c => c.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        mapper.Setup(m => m.Map<SearchResultCollection>(dto))
            .Throws(new Exception());

        // Act
        var result = await sut.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeEquivalentTo(new SearchResultCollection());
    }

    [Theory]
    [AutoMoqData]
    internal async Task BaseClient_SearchAsync_Database_Fails_Returns_EmptyCollection(
        [Frozen] Mock<IDbClient> client,
        [Frozen] Mock<IMapper> mapper,
        SearchResultDtoCollection dto,
        BaseClient sut)
    {
        // Arrange
        client
            .Setup(c => c.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        // Act
        var result = await sut.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeEquivalentTo(new SearchResultCollection());
    }
}
