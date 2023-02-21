using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Domain;
using SoundForest.Framework.Application.Requests;
using SoundForest.UnitTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Commands;
public sealed class UpsertExportCommandTests
{
    [Theory]
    [AutoMoqData]
    internal async Task UpsertExportCommandHandler_Handle_Succeeds(
        [Frozen] Mock<IClient> client,
        Export export,
        UpsertExportCommand cmd,
        UpsertExportCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertPropertiesAsync(It.IsNotNull<string>(), It.IsNotNull<IDictionary<string, object>>(), It.IsNotNull<CancellationToken>()))
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
    internal async Task UpsertExportCommandHandler_Handle_NoResults_ReturnsNotFound(
        [Frozen] Mock<IClient> client,
        UpsertExportCommand query,
        UpsertExportCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertPropertiesAsync(It.IsNotNull<string>(), It.IsNotNull<IDictionary<string, object>>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Export>());

        // Act
        var result = await sut.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(Result<Export>.NotFoundResult("Sorry, we could not save this export :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task UpsertExportCommandHandler_Handle_Throws_Returns_InternalError(
        [Frozen] Mock<IClient> client,
        UpsertExportCommand query,
        UpsertExportCommandHandler sut)
    {
        // Arrange
        client
            .Setup(c => c.UpsertPropertiesAsync(It.IsNotNull<string>(), It.IsNotNull<IDictionary<string, object>>(), It.IsNotNull<CancellationToken>()))
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
