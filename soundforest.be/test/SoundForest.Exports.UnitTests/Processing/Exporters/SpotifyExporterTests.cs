using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Exporters;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
using SoundForest.UnitTests.Common;
using SpotifyAPI.Web;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class SpotifyExporterTests
{
    [Theory]
    [AutoMoqData]
    internal async Task SoundtrackParser_HappyFlow(
            ILogger<SpotifyExporter> logger,
            Mock<ISearchService> searchService,
            Mock<IPlaylistService> playlistService,
            IEnumerable<Soundtrack?> songs,
            IEnumerable<FullTrack> tracks,
            string playlistId,
            string username,
            string name)
    {
        // Arrange
        searchService
            .Setup(s => s.SearchTracks(It.IsAny<IEnumerable<Soundtrack>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracks);

        playlistService
            .Setup(p => p.SaveAsync(It.IsAny<IEnumerable<FullTrack>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlistId);

        var sut = new SpotifyExporter(logger, searchService.Object, playlistService.Object);

        // Act
        var result = await sut.ExportAsync(songs, username, name, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ExternalId.Should().Be(playlistId);
        }
    }
}
