using FluentAssertions;
using SoundForest.Playlists.Management.Application.Queries;
using SoundForest.Playlists.Management.Application.Validators;
using System.Collections.Generic;
using Xunit;

namespace SoundForest.Playlists.UnitTests.Management.Validators;
public sealed class PlaylistByIdQueryValidatorTests
{
    public static IEnumerable<object[]> ValidQueries
    {
        get
        {
            yield return new object[] { new PlaylistByIdQuery("tt1234567") };
            yield return new object[] { new PlaylistByIdQuery("tt12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(ValidQueries))]
    public void Validator_Succeeds(PlaylistByIdQuery query)
    {
        // Arrange
        var sut = new PlaylistByIdQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> InvalidQueries
    {
        get
        {
            yield return new object[] { new PlaylistByIdQuery("") };
            yield return new object[] { new PlaylistByIdQuery(" ") };
            yield return new object[] { new PlaylistByIdQuery("   ") };
            yield return new object[] { new PlaylistByIdQuery("tt123456789") };
            yield return new object[] { new PlaylistByIdQuery("tt123456") };
            yield return new object[] { new PlaylistByIdQuery("N/A") };
            yield return new object[] { new PlaylistByIdQuery("aa12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public void Validator_Fails(PlaylistByIdQuery query)
    {
        // Arrange
        var sut = new PlaylistByIdQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
