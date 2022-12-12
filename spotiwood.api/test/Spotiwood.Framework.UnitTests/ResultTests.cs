using FluentAssertions;
using Moq;
using Spotiwood.Framework.Application.Errors;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.UnitTests.Common;
using System;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Framework.UnitTests;
public sealed class ResultTests
{
    [Theory]
    [AutoMoqData]
    public void Result_Success(string message)
    {
        // Act
        var result = Result<string>.SuccessResult(message);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().BeNull();
        result.DisplayMessage.Should().BeNull();
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(message);
    }

    [Theory]
    [AutoMoqData]
    public void Result_UserError(string message, IEnumerable<Error> errors)
    {
        // Act
        var result = Result<string>.UserErrorResult(message, errors);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().NotBeNull().And.BeEquivalentTo(errors);
        result.DisplayMessage.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo(message);
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    public void Result_ServerError(string message, IEnumerable<Error> errors)
    {
        // Act
        var result = Result<string>.ServerErrorResult(message, errors);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().NotBeNull().And.BeEquivalentTo(errors);
        result.DisplayMessage.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo(message);
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(500);
        result.Value.Should().BeNull();
    }


    [Theory]
    [AutoMoqData]
    public void Result_NotFoundError(string message, IEnumerable<Error> errors)
    {
        // Act
        var result = Result<string>.NotFoundResult(message);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().BeNull();
        result.DisplayMessage.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo(message);
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Result_Switch_Success_Hit()
    {
        // Arrange
        var sut = Result<string>.SuccessResult(It.IsAny<string>());

        // Act
        var action = () => sut.Switch(
            success: (s, v) =>
            {

            },
            failure: (s, m, e) =>
            {
                throw new Exception();
            });

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Result_Switch_Failure_Hit()
    {
        // Arrange
        var sut = Result<string>.ServerErrorResult(It.IsNotNull<string>(), It.IsNotNull<IEnumerable<Error>>());

        // Act
        var action = () => sut.Switch(
            success: (s, v) =>
            {
                throw new Exception();
            },
            failure: (s, m, e) =>
            {

            });

        // Assert
        action.Should().NotThrow();
    }
}
