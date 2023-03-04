using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using SoundForest.Exports.Processing.Infrastructure.Parsers;
using SoundForest.UnitTests.Common;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class DocumentLoaderTests
{
    private static string Html = "<html><head></head><body>Test</body></html>";

    [Theory]
    [AutoMoqData]
    internal async Task ImdbDocumentLoader_HappyFlow(
        ILogger<ImdbDocumentLoader> logger,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        IEnumerable<string> ids)
    {
        // Arrange
        handler.SetupAnyRequest()
            .ReturnsResponse(Html);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new ImdbDocumentLoader(logger, client);

        // Act
        var result = await sut.LoadDocumentsAsync(ids, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNullOrEmpty();
            result!.Length.Should().Be(ids.Count());
        }
    }
}
