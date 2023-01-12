using FluentAssertions;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Application.Validators;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.Validators;
public sealed class GetPlaylistsQueryValidatorTests
{
    public static IEnumerable<object[]> ValidQueries
    {
        get
        {
            yield return new object[] { new GetPlaylistsQuery(1, 1) };
            yield return new object[] { new GetPlaylistsQuery(null, null) };
            yield return new object[] { new GetPlaylistsQuery(10, null) };
            yield return new object[] { new GetPlaylistsQuery(null, 10) };
        }
    }

    [Theory]
    [MemberData(nameof(ValidQueries))]
    public void Validator_Succeeds(GetPlaylistsQuery query)
    {
        // Arrange
        var sut = new GetPlaylistsQueryValidator();

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
            yield return new object[] { new GetPlaylistsQuery(0, 0) };
            yield return new object[] { new GetPlaylistsQuery(-1, -1) };
            yield return new object[] { new GetPlaylistsQuery(null, -1) };
            yield return new object[] { new GetPlaylistsQuery(0, null) };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public void Validator_Fails(GetPlaylistsQuery query)
    {
        // Arrange
        var sut = new GetPlaylistsQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
