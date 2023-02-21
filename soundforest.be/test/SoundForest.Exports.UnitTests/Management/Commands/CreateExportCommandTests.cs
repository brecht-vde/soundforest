using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Requests;
using SoundForest.UnitTests.Common;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Commands;
public sealed class CreateExportCommandTests
{
    [Theory]
    [AutoMoqData]
    internal async Task CreateExportCommandHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        Export export,
        CreateExportCommand cmd,
        CreateExportCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertAsync(It.IsNotNull<Export>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(export);

        // Act
        var result = await sut.Handle(cmd, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Export>.SuccessResult(export));
    }

    [Theory]
    [AutoMoqData]
    internal async Task CreateExportCommandHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        CreateExportCommand cmd,
        CreateExportCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertAsync(It.IsNotNull<Export>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Export>());

        // Act
        var result = await sut.Handle(cmd, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Export>.NotFoundResult("Sorry, we could not save this export :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task CreateExportCommandHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        CreateExportCommand cmd,
        CreateExportCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertAsync(It.IsNotNull<Export>(), It.IsNotNull<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await sut.Handle(cmd, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
        }
    }
}
