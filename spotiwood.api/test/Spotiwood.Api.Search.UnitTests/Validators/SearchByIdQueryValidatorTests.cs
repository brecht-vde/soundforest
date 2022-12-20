using FluentAssertions;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Api.Search.Application.Validators;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Api.Search.UnitTests.Validators;
public sealed class SearchByIdQueryValidatorTests
{
    public static IEnumerable<object[]> ValidQueries
    {
        get
        {
            yield return new object[] { new SearchByIdQuery("tt1234567") };
            yield return new object[] { new SearchByIdQuery("tt12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(ValidQueries))]
    public void Validator_Succeeds(SearchByIdQuery query)
    {
        // Arrange
        var sut = new SearchByIdQueryValidator();

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
            yield return new object[] { new SearchByIdQuery("") };
            yield return new object[] { new SearchByIdQuery(" ") };
            yield return new object[] { new SearchByIdQuery("   ") };
            yield return new object[] { new SearchByIdQuery("tt123456789") };
            yield return new object[] { new SearchByIdQuery("tt123456") };
            yield return new object[] { new SearchByIdQuery("N/A") };
            yield return new object[] { new SearchByIdQuery("aa12345678") };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public void Validator_Fails(SearchByIdQuery query)
    {
        // Arrange
        var sut = new SearchByIdQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
