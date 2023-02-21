using FluentAssertions;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Validators;
public sealed class UpsertExportCommandValidatorTests
{
    public static IEnumerable<object[]> ValidCommands
    {
        get
        {
            yield return new object[] { new UpsertExportCommand("tt1234567", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand("tt12345678", new Dictionary<string, object>() { { "Key", "Value" } }) };
        }
    }

    [Theory]
    [MemberData(nameof(ValidCommands))]
    public void Validator_Succeeds(UpsertExportCommand cmd)
    {
        // Arrange
        var sut = new UpsertExportCommandValidator();

        // Act
        var result = sut.Validate(cmd);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> InvalidCommands
    {
        get
        {
            yield return new object[] { new UpsertExportCommand("", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand(" ", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand("   ", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand("tt123456789", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand("tt123456", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand("N/A", new Dictionary<string, object>() { { "Key", "Value" } }) };
            yield return new object[] { new UpsertExportCommand("aa12345678", new Dictionary<string, object>() { { "Key", "Value" } }) };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCommands))]
    public void Validator_Fails(UpsertExportCommand cmd)
    {
        // Arrange
        var sut = new UpsertExportCommandValidator();

        // Act
        var result = sut.Validate(cmd);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
