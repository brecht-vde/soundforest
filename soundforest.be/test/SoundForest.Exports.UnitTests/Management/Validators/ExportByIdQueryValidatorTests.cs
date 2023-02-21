using FluentAssertions;
using SoundForest.Exports.Management.Application.Queries;
using SoundForest.Exports.Management.Application.Validators;
using Xunit;

namespace SoundForest.Exports.UnitTests.Management.Validators;
public sealed class ExportByIdQueryValidatorTests
{
    public static IEnumerable<object[]> ValidCommands
    {
        get
        {
            yield return new object[] { new ExportByIdQuery("tt1234567") };
            yield return new object[] { new ExportByIdQuery("tt12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(ValidCommands))]
    public void Validator_Succeeds(ExportByIdQuery query)
    {
        // Arrange
        var sut = new ExportByIdQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> InvalidCommands
    {
        get
        {
            yield return new object[] { new ExportByIdQuery("") };
            yield return new object[] { new ExportByIdQuery(" ") };
            yield return new object[] { new ExportByIdQuery("   ") };
            yield return new object[] { new ExportByIdQuery("tt123456789") };
            yield return new object[] { new ExportByIdQuery("tt123456") };
            yield return new object[] { new ExportByIdQuery("N/A") };
            yield return new object[] { new ExportByIdQuery("aa12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCommands))]
    public void Validator_Fails(ExportByIdQuery query)
    {
        // Arrange
        var sut = new ExportByIdQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
