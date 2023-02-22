﻿using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SoundForest.Exports.Processing.Application.Commands;
using SoundForest.Exports.Processing.Application.Exporters;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Framework.Application.Requests;
using SoundForest.UnitTests.Common;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Commands;
public sealed class ProcessExportCommandTests
{
    [Theory]
    [AutoMoqData]
    internal async Task ProcessExportCommandHandler_Handle_Succeeds(
        [Frozen] Mock<IExporter<IEnumerable<Soundtrack>?>> exporter,
        ProcessExportCommand cmd,
        ProcessExportCommandHandler sut,
        string externalId,
        string[] log)
    {
        // Arrange
        exporter
            .Setup(e => e.ExportAsync(It.IsNotNull<IEnumerable<Soundtrack>?>(), It.IsNotNull<string>(), It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync((externalId, log));

        // Act
        var result = await sut.Handle(cmd, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<Result<string>>(Result<string>.SuccessResult(externalId));
    }

    [Theory]
    [AutoMoqData]
    internal async Task ProcessExportCommandHandler_Handle_NotExporterReturnsNull(
        [Frozen] Mock<IExporter<IEnumerable<Soundtrack>?>> exporter,
        ProcessExportCommand cmd,
        ProcessExportCommandHandler sut)
    {
        // Arrange
        exporter
            .Setup(e => e.ExportAsync(It.IsNotNull<IEnumerable<Soundtrack>?>(), It.IsNotNull<string>(), It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
            .ReturnsAsync((It.IsAny<string>(), It.IsAny<string[]>()));

        // Act
        var result = await sut.Handle(cmd, It.IsAny<CancellationToken>());

        // Assert
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo<Result<string>>(Result<string>.NotFoundResult("Sorry, could not export the playlist. :(."));
    }

    [Theory]
    [AutoMoqData]
    internal async Task ProcessExportCommandHandler_Handle_ThrowsReturnsNull(
        [Frozen] Mock<IExporter<IEnumerable<Soundtrack>?>> exporter,
        ProcessExportCommand cmd,
        ProcessExportCommandHandler sut)
    {
        // Arrange
        exporter
            .Setup(e => e.ExportAsync(It.IsNotNull<IEnumerable<Soundtrack>?>(), It.IsNotNull<string>(), It.IsNotNull<string>(), It.IsNotNull<CancellationToken>()))
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
