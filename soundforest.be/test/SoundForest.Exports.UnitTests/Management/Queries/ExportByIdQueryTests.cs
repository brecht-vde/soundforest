using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Application.Queries;
using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Requests;
using SoundForest.UnitTests.Common;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Queries;
public sealed class ExportByIdQueryTests
{
    [Theory]
    [AutoMoqData]
    internal async Task ExportByIdQueryHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        Export export,
        ExportByIdQuery query,
        ExportByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(export);

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Export>.SuccessResult(export));
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportByIdQueryHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        ExportByIdQuery query,
        ExportByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Export>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Export>.NotFoundResult("Sorry, we did not find this export :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task ExportByIdQueryHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        ExportByIdQuery query,
        ExportByIdQueryHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.SingleAsync(It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
        }
    }
}
