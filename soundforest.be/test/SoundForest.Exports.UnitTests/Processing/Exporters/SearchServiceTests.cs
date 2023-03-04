using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
using SoundForest.UnitTests.Common;
using SpotifyAPI.Web;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class SearchServiceTests
{
    [Theory]
    [AutoMoqData]
    internal async Task SearchService_HappyFlow(
        ILogger<SearchService> logger,
        Mock<ITokenService> token,
        Mock<ISpotifyClientFactory> factory,
        Mock<ISearchClient> search,
        SearchResponse response,
        string tokenValue,
        IEnumerable<Soundtrack> tracks
        )
    {
        // Arrange
        token.Setup(t => t.GetM2mAccesToken(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenValue);

        search.Setup(s => s.Item(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        factory.Setup(f => f.Create<ISearchClient>(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(search.Object);

        var sut = new SearchService(logger, token.Object, factory.Object);

        // Act
        var result = sut.SearchTracks(tracks, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
    }
}
