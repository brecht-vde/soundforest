using FluentAssertions;
using SoundForest.Playlists.Management.Application.Queries;
using SoundForest.Playlists.Management.Application.Validators;
using System.Collections.Generic;
using Xunit;

namespace SoundForest.Playlists.UnitTests.Management.Validators;
public sealed class PlaylistsQueryValidatorTests
{
    public static IEnumerable<object[]> ValidQueries
    {
        get
        {
            yield return new object[] { new PlaylistsQuery(1, 1) };
            yield return new object[] { new PlaylistsQuery(null, null) };
            yield return new object[] { new PlaylistsQuery(10, null) };
            yield return new object[] { new PlaylistsQuery(null, 10) };
        }
    }

    [Theory]
    [MemberData(nameof(ValidQueries))]
    public void Validator_Succeeds(PlaylistsQuery query)
    {
        // Arrange
        var sut = new PlaylistsQueryValidator();

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
            yield return new object[] { new PlaylistsQuery(0, 0) };
            yield return new object[] { new PlaylistsQuery(-1, -1) };
            yield return new object[] { new PlaylistsQuery(null, -1) };
            yield return new object[] { new PlaylistsQuery(0, null) };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public void Validator_Fails(PlaylistsQuery query)
    {
        // Arrange
        var sut = new PlaylistsQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
