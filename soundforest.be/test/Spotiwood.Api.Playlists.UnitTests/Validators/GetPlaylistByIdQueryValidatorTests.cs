using FluentAssertions;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Application.Validators;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.Validators;
public sealed class GetPlaylistByIdQueryValidatorTests
{
    public static IEnumerable<object[]> ValidQueries
    {
        get
        {
            yield return new object[] { new GetPlaylistByIdQuery("tt1234567") };
            yield return new object[] { new GetPlaylistByIdQuery("tt12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(ValidQueries))]
    public void Validator_Succeeds(GetPlaylistByIdQuery query)
    {
        // Arrange
        var sut = new GetPlaylistByIdQueryValidator();

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
            yield return new object[] { new GetPlaylistByIdQuery("") };
            yield return new object[] { new GetPlaylistByIdQuery(" ") };
            yield return new object[] { new GetPlaylistByIdQuery("   ") };
            yield return new object[] { new GetPlaylistByIdQuery("tt123456789") };
            yield return new object[] { new GetPlaylistByIdQuery("tt123456") };
            yield return new object[] { new GetPlaylistByIdQuery("N/A") };
            yield return new object[] { new GetPlaylistByIdQuery("aa12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public void Validator_Fails(GetPlaylistByIdQuery query)
    {
        // Arrange
        var sut = new GetPlaylistByIdQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
