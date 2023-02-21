using FluentAssertions;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Application.Validators;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Validators;
public sealed class CreateExportCommandValidatorTests
{
    public static IEnumerable<object[]> ValidCommands
    {
        get
        {
            yield return new object[] { new CreateExportCommand("tt1234567", "Name", "Username") };
            yield return new object[] { new CreateExportCommand("tt12345678", "Name", "Username") };
        }
    }

    [Theory]
    [MemberData(nameof(ValidCommands))]
    public void Validator_Succeeds(CreateExportCommand cmd)
    {
        // Arrange
        var sut = new CreateExportCommandValidator();

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
            yield return new object[] { new CreateExportCommand("", "Name", "Username") };
            yield return new object[] { new CreateExportCommand(" ", "Name", "Username") };
            yield return new object[] { new CreateExportCommand("   ", "Name", "Username") };
            yield return new object[] { new CreateExportCommand("tt123456789", "Name", "Username") };
            yield return new object[] { new CreateExportCommand("tt123456", "Name", "Username") };
            yield return new object[] { new CreateExportCommand("N/A", "Name", "Username") };
            yield return new object[] { new CreateExportCommand("aa12345678", "Name", "Username") };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCommands))]
    public void Validator_Fails(CreateExportCommand cmd)
    {
        // Arrange
        var sut = new CreateExportCommandValidator();

        // Act
        var result = sut.Validate(cmd);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
