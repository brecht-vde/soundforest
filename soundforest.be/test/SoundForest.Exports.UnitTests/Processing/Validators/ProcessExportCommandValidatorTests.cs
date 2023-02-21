using FluentAssertions;
using SoundForest.Exports.Processing.Application.Commands;
using SoundForest.Exports.Processing.Application.Validators;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Validators;
public sealed class ProcessExportCommandValidatorTests
{
    public static IEnumerable<object[]> ValidCommands
    {
        get
        {
            yield return new object[] { new ProcessExportCommand("tt1234567", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand("tt12345678", "Name", "Username") };
        }
    }

    [Theory]
    [MemberData(nameof(ValidCommands))]
    public void Validator_Succeeds(ProcessExportCommand cmd)
    {
        // Arrange
        var sut = new ProcessExportCommandValidator();

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
            yield return new object[] { new ProcessExportCommand("", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand(" ", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand("   ", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand("tt123456789", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand("tt123456", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand("N/A", "Name", "Username") };
            yield return new object[] { new ProcessExportCommand("aa12345678", "Name", "Username") };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCommands))]
    public void Validator_Fails(ProcessExportCommand cmd)
    {
        // Arrange
        var sut = new ProcessExportCommandValidator();

        // Act
        var result = sut.Validate(cmd);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
